using Common.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace LD.Common.Queue.Subscriber
{
    /// <summary>
    /// Interface for builder
    /// </summary>
    public interface IRabbitMQSubscriberBuilder
    {
        /// <summary>
        /// Set credentials for rabbit server, required
        /// </summary>
        IRabbitMQSubscriberBuilder WithCredentials(string username, string password);

        /// <summary>
        /// Subscribe to a Queue
        /// </summary>
        /// <param name="queueName"></param>
        IRabbitMQSubscriberBuilder WithQueue(string queueName);

        /// <summary>
        /// Enable auto acknowledgement, default requires manual acknowledgement of messages
        /// If this is enabled, once messages are delivered they cannot be redelivered on failure
        /// Only do this if you don't care about lost messages
        /// </summary>
        /// <returns></returns>
        IRabbitMQSubscriberBuilder WithAutoAck();

        /// <summary>
        /// Disables confirms when republishing messages.
        /// WARNING: If this is disabled, then we don't confirm whether the publish succeeded.
        ///          This can lead to conditions where a message is ACKed, but the republish fails.
        ///          The only reason to disable is if you don't care about republishes
        /// </summary>
        IRabbitMQSubscriberBuilder DisableRepublishConfirms();

        /// <summary>
        /// Set how many connections to the rabbit server you will make (== tcp connections)
        /// </summary>
        /// <param name="numConns">defaults to 1</param>
        IRabbitMQSubscriberBuilder WithConnections(int numConns = 1);

        /// <summary>
        /// Set how many channels to enable per connection.
        /// Each channel will get messages in its own "thread", but the channels are fed by a single receiver on the connection
        /// </summary>
        /// <param name="numChannels">defaults to 1</param>
        IRabbitMQSubscriberBuilder WithChannelsPerConnection(int numChannels = 1);

        /// <summary>
        /// Set the timeout for waiting for a republish confirmation.
        /// Note: This blocks acknowledgement, and if times out, it will fail and requeue
        /// </summary>
        /// <param name="timeout">defaults to 15 seconds</param>
        IRabbitMQSubscriberBuilder WithRepublishConfirmTimeout(TimeSpan timeout);

        /// <summary>
        /// Set the callback to receive messages, this or WithExtendedCallback required
        /// This callback only provides the message body
        /// </summary>
        /// <param name="callback">function that will receive the message, and return response</param>
        IRabbitMQSubscriberBuilder WithSimpleCallback(Func<byte[], CallbackResponse> callback);

        /// <summary>
        /// Set the callback to receive messages, this or WithSimpleCallback required
        /// This callback provides the entire message object and allows for republishing
        /// </summary>
        /// <param name="callback">function that will receive the message, and determine the next step.  Republishing is supported, the
        /// BasicDeliverEventArgs is mutable for republishing.
        /// </param>
        IRabbitMQSubscriberBuilder WithExtendedCallback(Func<BasicDeliverEventArgs, ExtendedCallbackResponse> callback);

        /// <summary>
        /// Build the subscriber
        /// </summary>
        /// <returns>a subscriber that is subcribed</returns>
        IRabbitMQSubscriber Build();
    }

    /// <summary>
    /// Builder for a RabbitMQ subscriber
    ///
    /// Use builder pattern
    /// <code>
    ///     new RabbitMQSubscriberBuilder("queue", 1234)
    ///                 .WithCredentials("a", "b")
    ///                 .WithQueue("x")
    ///                 .Build()
    /// </code>
    /// </summary>
    public class RabbitMQSubscriberBuilder : IRabbitMQSubscriberBuilder
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        private readonly string _host;
        private readonly int _port;

        private string _username;
        private string _password;
        private string _queueName;
        private bool _autoAck;
        private Func<byte[], CallbackResponse> _simpleCallback;
        private Func<BasicDeliverEventArgs, ExtendedCallbackResponse> _extendedCallback;
        private int _numConns = 1;
        private int _numChannels = 1;
        private bool _disableConfirms;
        private TimeSpan _confirmTimeout = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Construct a new subscriber
        /// </summary>
        /// <param name="host">hostname for rabbit server</param>
        /// <param name="port">port for rabbit server</param>
        public RabbitMQSubscriberBuilder(string host, int port)
        {
            _host = host;
            _port = port;
        }

        /// <summary>
        /// Construct from settings
        /// </summary>
        /// <param name="settings"></param>
        public RabbitMQSubscriberBuilder(RabbitMQSubscriberSettings settings) : this(settings.Host, settings.Port)
        {
            WithCredentials(settings.Username, settings.Password)
                .WithQueue(settings.QueueName)
                .WithConnections(settings.Connections)
                .WithChannelsPerConnection(settings.ChannelsPerConnection);

            if (settings.RepublishConfirmsTimeout.HasValue)
                WithRepublishConfirmTimeout(settings.RepublishConfirmsTimeout.Value);
            if (settings.DisableRepublishConfirms)
                DisableRepublishConfirms();
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithCredentials(string username, string password)
        {
            _username = username;
            _password = password;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithQueue(string queueName)
        {
            _queueName = queueName;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithAutoAck()
        {
            log.Info("Warning: Will Auto-Acknowledge messages");
            _autoAck = true;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithConnections(int numConns = 1)
        {
            _numConns = numConns;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithChannelsPerConnection(int numChannels = 1)
        {
            _numChannels = numChannels;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithRepublishConfirmTimeout(TimeSpan timeout)
        {
            _confirmTimeout = timeout;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithSimpleCallback(Func<byte[], CallbackResponse> callback)
        {
            if (_extendedCallback != null)
                throw new ArgumentException("cannot specify both simple and extended callbacks");

            _simpleCallback = callback;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder WithExtendedCallback(Func<BasicDeliverEventArgs, ExtendedCallbackResponse> callback)
        {
            if (_simpleCallback != null)
                throw new ArgumentException("cannot specify both simple and extended callbacks");

            _extendedCallback = callback;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriberBuilder DisableRepublishConfirms()
        {
            log.Info("Warning: Disabling republish confirms, republishes may be lost");
            _disableConfirms = true;
            return this;
        }

        /// <see cref="IRabbitMQSubscriberBuilder"/>
        public IRabbitMQSubscriber Build()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _host,
                Port = _port,
                UserName = _username,
                Password = _password,
                AutomaticRecoveryEnabled = true,
                UseBackgroundThreadsForIO = true    // rabbit will sometimes leave IO threads running, this allows the process to exit even if the threads are hung
            };

            var connChannels = new Dictionary<IConnection, List<IModel>>();

            try
            {
                if (_queueName == null)
                    throw new ArgumentException("queue not specified");

                for (int i = 0; i < _numConns; i++)
                {
                    var channels = new List<IModel>(_numChannels);

                    var connection = factory.CreateConnection();
                    connChannels.Add(connection, channels);

                    for (int j = 0; j < _numChannels; j++)
                    {
                        channels.Add(connection.CreateModel());
                    }
                }

                if (_simpleCallback != null)
                    return new RabbitMQSubscriber(_queueName, connChannels, _simpleCallback, _autoAck, _disableConfirms, _confirmTimeout);
                else if (_extendedCallback != null)
                    return new RabbitMQSubscriber(_queueName, connChannels, _extendedCallback, _autoAck, _disableConfirms, _confirmTimeout);
                else
                    throw new ArgumentException("callback not specified");
            }
            catch (Exception)
            {
                foreach (var cc in connChannels)
                {
                    foreach (var c in cc.Value)
                        c.Dispose();
                    cc.Key.Dispose();
                }
                throw;
            }
        }
    }
}