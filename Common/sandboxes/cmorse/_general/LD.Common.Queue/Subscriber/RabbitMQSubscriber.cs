using Common.Logging;
using LD.Common.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace LD.Common.Queue.Subscriber
{
    /// <summary>
    /// Subscriber that will listen to a queue
    /// </summary>
    internal class RabbitMQSubscriber : IRabbitMQSubscriber
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        private readonly string _queue;
        private readonly Dictionary<IConnection, List<IModel>> _connChannels;
        private readonly bool _autoAck;
        private readonly Func<byte[], CallbackResponse> _simpleCallback;
        private readonly Func<BasicDeliverEventArgs, ExtendedCallbackResponse> _extendedCallback;
        private readonly bool _isSimple;
        private readonly bool _disableConfirms;
        private readonly TimeSpan _confirmTimeout;

        private readonly IDictionary<string, EventingBasicConsumer> _consumers = new Dictionary<string, EventingBasicConsumer>();
        private readonly CountdownEvent _endEvent;
        private volatile bool _shutdown = false;

        private RabbitMQSubscriber(string queue, Dictionary<IConnection, List<IModel>> connChannels, bool autoAck, bool disableConfirms, TimeSpan confirmTimeout)
        {
            _connChannels = connChannels;
            _queue = queue ?? "";
            _autoAck = autoAck;
            _disableConfirms = disableConfirms;
            _confirmTimeout = confirmTimeout;

            _endEvent = new CountdownEvent(1);
        }

        internal RabbitMQSubscriber(string queue, Dictionary<IConnection, List<IModel>> connChannels, Func<byte[], CallbackResponse> callback, bool autoAck, bool disableConfirms, TimeSpan confirmTimeout)
            : this(queue, connChannels, autoAck, disableConfirms, confirmTimeout)
        {
            _simpleCallback = callback;
            _isSimple = true;

            initConsumers(this);
        }

        internal RabbitMQSubscriber(string queue, Dictionary<IConnection, List<IModel>> connChannels, Func<BasicDeliverEventArgs, ExtendedCallbackResponse> callback, bool autoAck, bool disableConfirms, TimeSpan confirmTimeout)
            : this(queue, connChannels, autoAck, disableConfirms, confirmTimeout)
        {
            _extendedCallback = callback;
            _isSimple = false;

            initConsumers(this);
        }

        private static void initConsumers(RabbitMQSubscriber sub)
        {
            log.Debug($"Creating subscribers for {sub._queue}, conns={sub._connChannels.Count}, channels={sub._connChannels.Values.First().Count}");

            foreach (var channelList in sub._connChannels.Values)
            {
                foreach (var channel in channelList)
                {
                    if (!sub._disableConfirms)
                        channel.ConfirmSelect();

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += sub.receive;
                    consumer.Unregistered += sub.cancelok;
                    channel.BasicQos(0, 1, false);
                    var tag = channel.BasicConsume(queue: sub._queue,
                                                   noAck: sub._autoAck,
                                                   consumer: consumer);
                    sub._consumers.Add(tag, consumer);
                    sub._endEvent.AddCount();
                }
            }
        }

        private void cancelok(object model, ConsumerEventArgs ea)
        {
            log.Debug($"Consumer {ea.ConsumerTag} unregistered successfully");

            _endEvent.Signal();
        }

        private void receive(object model, BasicDeliverEventArgs ea)
        {
            EventingBasicConsumer consumer = model as EventingBasicConsumer;

            if (_shutdown)
            {
                // we're shutting down, requeue any that come in
                log.TraceFormat("got a message during shutdown, requeing: {0}", consumer.ConsumerTag);
                if (consumer.Model.IsOpen)
                    consumer.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                return;
            }

            receiveImpl(consumer, ea);
        }

        private void receiveImpl(EventingBasicConsumer consumer, BasicDeliverEventArgs ea)
        {
            CallbackResponse resp = CallbackResponse.ReQueue;
            ExtendedCallbackResponse extResp = null;

            try
            {
                log.TraceFormat("consumer: {0}", consumer.ConsumerTag);
                using (new TimeUtil.TimeLogger($"queue recv", log))
                {
                    if (_isSimple)
                        resp = _simpleCallback.Invoke(ea.Body);
                    else
                    {
                        extResp = _extendedCallback.Invoke(ea);
                        resp = extResp.Response;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("caught in queue callback, will requeue", e);
                resp = CallbackResponse.ReQueue;
            }

            if (_autoAck)
            {
                log.Debug($"queue recv AutoAck: Response was {resp}");
                return;
            }

            log.Debug($"queue recv Response: {resp}");

            switch (resp)
            {
                case CallbackResponse.Acknowledge:
                    if (!_isSimple)
                        republish(ea, consumer, extResp);

                    consumer.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    break;

                case CallbackResponse.ReQueue:
                    consumer.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    break;

                case CallbackResponse.Dead:
                    consumer.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    break;
            }
        }

        private void republish(BasicDeliverEventArgs ea, EventingBasicConsumer consumer, ExtendedCallbackResponse resp)
        {
            if (resp.Republish || resp.PublishToQueue != null)
            {
                log.DebugFormat("Republishing message to {0} queue", resp.PublishToQueue ?? "original");
                republish(ea, consumer, "", resp.PublishToQueue ?? _queue);
            }
            else if (resp.PublishToExchange != null)
            {
                log.DebugFormat("Republishing message to {0} exchange", resp.PublishToExchange);
                republish(ea, consumer, resp.PublishToExchange, ea.RoutingKey);
            }
        }

        private void republish(BasicDeliverEventArgs ea, EventingBasicConsumer consumer, string exchange, string routingKey)
        {
            ea.BasicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            consumer.Model.BasicPublish(exchange: exchange,
                                        routingKey: routingKey,
                                        basicProperties: ea.BasicProperties,
                                        body: ea.Body);

            if (!_disableConfirms)
            {
                try
                {
                    consumer.Model.WaitForConfirmsOrDie(_confirmTimeout);
                }
                catch (IOException e)
                {
                    log.Warn($"republish failed or timed out! {e.Message}");
                    throw;
                }
            }
        }

        public void Stop(TimeSpan? timeout = null)
        {
            log.Info($"Stopping and Waiting for all RabbitMQ subscribers for {_queue} timeout = {timeout}");

            // signal shutdown so subscribers stop processing messages
            _shutdown = true;

            // cancel consumers
            foreach (var kv in _consumers)
            {
                kv.Value.Model.BasicCancel(kv.Key);
            }

            // signal the final countdown
            _endEvent.Signal();

            // wait for all consumers to cancel
            DateTime start = DateTime.UtcNow;
            TimeSpan? timeoutLeft = null;
            if (timeout.HasValue)
            {
                if (!_endEvent.Wait(timeout.Value))
                    log.Warn($"Not all consumers finished, count = {_endEvent.CurrentCount}");

                timeoutLeft = timeout.Value - (DateTime.UtcNow - start);
                if (timeoutLeft < TimeSpan.Zero)
                    timeoutLeft = TimeSpan.FromMilliseconds(250);
            }
            else
                _endEvent.Wait();

            var conn = _connChannels.First().Key;

            safeClose(conn, timeoutLeft);

            log.Info($"RabbitMQ subscribers stopped for {_queue}");
        }

        public void Dispose()
        {
            log.Info($"Shutting down RabbitMQ subscriber for {_queue}");

            var conn = _connChannels.First().Key;

            safeClose(conn, TimeSpan.FromSeconds(1));

            log.Info($"RabbitMQ subscriber shut down");
        }

        private void safeClose(IConnection c, TimeSpan? timeout)
        {
            foreach (var cc in _connChannels[c])
                safeClose(cc);

            try
            {
                if (c.IsOpen)
                {
                    if (timeout.HasValue)
                        c.Close((int)timeout.Value.TotalSeconds);
                    else
                        c.Close();
                }
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
            {
                // ignore
            }
            catch (Exception e)
            {
                log.Warn($"caught exception during connection close {e.GetType().FullName}: {e.Message}");
            }

            try
            {
                c.Dispose();
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                // ignore
            }
            catch (Exception e)
            {
                log.Warn($"caught exception during connection dispose {e.GetType().FullName}: {e.Message}");
            }
        }

        private void safeClose(IModel c)
        {
            try
            {
                c.Close();
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
            {
                // ignore
            }
            catch (Exception e)
            {
                log.Warn($"caught exception during channel close {e.GetType().FullName}: {e.Message}");
            }
        }
    }
}
