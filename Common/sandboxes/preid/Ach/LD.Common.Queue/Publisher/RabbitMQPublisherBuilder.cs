using RabbitMQ.Client;
using System;

namespace LD.Common.Queue.Publisher
{
    /// <summary>
    /// Builder for a RabbitMQ publisher
    ///
    /// Use builder pattern
    /// <code>
    ///     new RabbitMQPublisherBuilder("queue", 1234)
    ///                 .WithCredentials("a", "b")
    ///                 .WithExchange("x")
    ///                 .Build()
    /// </code>
    /// </summary>
    public class RabbitMQPublisherBuilder
    {
        private readonly string _host;
        private readonly int _port;

        private string _username = null;
        private string _password = null;
        private string _queueName = null;
        private string _exchangeName = null;
        private bool _persistent = true;
        private bool _testMode = false;

        /// <summary>
        /// Construct a new builder
        /// </summary>
        /// <param name="host">hostname for rabbit server</param>
        /// <param name="port">port for rabbit server</param>
        public RabbitMQPublisherBuilder(string host, int port)
        {
            _host = host;
            _port = port;
        }

        /// <summary>
        /// Construct from settings
        /// </summary>
        /// <param name="settings"></param>
        public RabbitMQPublisherBuilder(RabbitMQPublisherSettings settings) : this(settings.Host, settings.Port)
        {
            WithCredentials(settings.Username, settings.Password);
        }

        /// <summary>
        /// Set credentials for rabbit server, required
        /// </summary>
        public RabbitMQPublisherBuilder WithCredentials(string username, string password)
        {
            _username = username;
            _password = password;
            return this;
        }

        /// <summary>
        /// Publish Directly to a Queue without an Exchange, (this or WithExchange() is required)
        /// </summary>
        /// <remarks>
        /// RabbitMQ prefers that all publishing go to Exchanges, and queues get bound to the exchange.
        /// However, it is possible to publish directly to a Queue.  This should only be used for private queues (i.e. not between components)
        /// </remarks>
        /// <param name="queueName"></param>
        public RabbitMQPublisherBuilder WithDirectQueue(string queueName)
        {
            if (_exchangeName != null)
                throw new ArgumentException("cannot publish to both queue and exchange");

            _queueName = queueName;
            return this;
        }

        /// <summary>
        /// Publish to an Exchange (this or WithDirectQueue() is required)
        /// </summary>
        /// <param name="exchangeName"></param>
        public RabbitMQPublisherBuilder WithExchange(string exchangeName)
        {
            if (_queueName != null)
                throw new ArgumentException("cannot publish to both queue and exchange");

            _exchangeName = exchangeName;
            return this;
        }

        /// <summary>
        /// Disable persistence of the published messages (in memory only)
        /// </summary>
        public RabbitMQPublisherBuilder WithoutPersistence()
        {
            _persistent = false;
            return this;
        }

        /// <summary>
        /// Enable persistence of the published messages (messages will be written to disk)
        /// This is on by default
        /// </summary>
        public RabbitMQPublisherBuilder WithPersistence()
        {
            _persistent = true;
            return this;
        }

        /// <summary>
        /// Enable test mode
        /// This if off by default
        /// </summary>
        public RabbitMQPublisherBuilder TestMode(bool enable)
        {
            _testMode = enable;
            return this;
        }

        /// <summary>
        /// Build the publisher
        /// </summary>
        /// <returns>a publisher ready to publish</returns>
        public IRabbitMQPublisher Build()
        {
            if (_testMode)
                return new TestRabbitMQPublisher(_exchangeName, _queueName);

            var factory = new ConnectionFactory()
            {
                HostName = _host,
                Port = _port,
                UserName = _username,
                Password = _password,
                AutomaticRecoveryEnabled = true
            };

            IConnection connection = null;
            IModel channel = null;

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                if (_queueName == null && _exchangeName == null)
                    throw new ArgumentException("queue or exchange not specified");

                return new RabbitMQPublisher(factory, _exchangeName, _queueName, _persistent, connection, channel);
            }
            catch (Exception)
            {
                if (channel != null)
                    channel.Dispose();
                if (connection != null)
                    connection.Dispose();
                throw;
            }
        }
    }
}