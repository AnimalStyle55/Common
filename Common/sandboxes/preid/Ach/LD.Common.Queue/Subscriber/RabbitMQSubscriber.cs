using Common.Logging;
using LD.Common.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
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

        private readonly CountdownEvent _endEvent;
        private volatile bool _shutdown = false;

        private RabbitMQSubscriber(string queue, Dictionary<IConnection, List<IModel>> connChannels, bool autoAck)
        {
            _connChannels = connChannels;
            _queue = queue ?? "";
            _autoAck = autoAck;

            _endEvent = new CountdownEvent(1);
        }

        internal RabbitMQSubscriber(string queue, Dictionary<IConnection, List<IModel>> connChannels, Func<byte[], CallbackResponse> callback, bool autoAck)
            : this(queue, connChannels, autoAck)
        {
            _simpleCallback = callback;
            _isSimple = true;

            initConsumers(this);
        }

        internal RabbitMQSubscriber(string queue, Dictionary<IConnection, List<IModel>> connChannels, Func<BasicDeliverEventArgs, ExtendedCallbackResponse> callback, bool autoAck)
            : this(queue, connChannels, autoAck)
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
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += sub.receive;
                    channel.BasicQos(0, 1, false);
                    channel.BasicConsume(queue: sub._queue,
                                         noAck: sub._autoAck,
                                         consumer: consumer);
                }
            }
        }

        private void receive(object model, BasicDeliverEventArgs ea)
        {
            EventingBasicConsumer consumer = model as EventingBasicConsumer;

            if (_shutdown)
            {
                // we're shutting down, requeue any that come in
                log.Debug($"got a message during shutdown, requeing: {consumer.ConsumerTag}");
                consumer.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                // note: basic.cancel is asynchronous and doesn't cancel right away, we might get another message or two
                consumer.Model.BasicCancel(consumer.ConsumerTag);
                return;
            }

            _endEvent.AddCount();
            try
            {
                receiveImpl(consumer, ea);

                if (_shutdown)
                {
                    // we're shutting down, might as well cancel this subscriber
                    log.TraceFormat("shutting down, cancelling consumer: {0}", consumer.ConsumerTag);
                    // note: basic.cancel is asynchronous and doesn't cancel right away, we might get another message or two
                    consumer.Model.BasicCancel(consumer.ConsumerTag);
                }
            }
            finally
            {
                _endEvent.Signal();
            }
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
                    consumer.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    if (!_isSimple)
                        republish(ea, consumer, extResp);

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

                ea.BasicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                consumer.Model.BasicPublish(exchange: "",
                                            routingKey: resp.PublishToQueue ?? _queue,
                                            basicProperties: ea.BasicProperties,
                                            body: ea.Body);
            }
            else if (resp.PublishToExchange != null)
            {
                log.DebugFormat("Republishing message to {0} exchange", resp.PublishToExchange);

                ea.BasicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                consumer.Model.BasicPublish(exchange: resp.PublishToExchange,
                                            routingKey: ea.RoutingKey,
                                            basicProperties: ea.BasicProperties,
                                            body: ea.Body);
            }
        }

        public void Stop(TimeSpan? timeout = null)
        {
            log.Info($"Stopping and Waiting for all RabbitMQ subscribers for {_queue}");

            // signal shutdown so subscribers stop processing messages
            _shutdown = true;

            // signal the final countdown
            _endEvent.Signal();

            // wait for all subscribers to finish
            if (timeout.HasValue)
            {
                if (!_endEvent.Wait(timeout.Value))
                    log.Warn($"Not all consumers finished, count = {_endEvent.CurrentCount}");
            }
            else
                _endEvent.Wait();

            log.Info($"RabbitMQ subscribers stopped for {_queue}");
        }

        public void Dispose()
        {
            log.Info($"Shutting down RabbitMQ subscriber for {_queue}");

            foreach (var cc in _connChannels)
            {
                foreach (var c in cc.Value)
                    c.Dispose();

                try
                {
                    // this can throw if subscribers are actively processing a message
                    cc.Key.Dispose();
                }
                catch (Exception e)
                {
                    log.Info($"caught exception during rabbit shutdown {e.GetType().FullName}");
                }
            }

            log.Info($"RabbitMQ subscriber shut down");
        }
    }
}
