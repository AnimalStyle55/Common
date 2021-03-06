﻿using Common.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD.Common.Queue.Publisher
{
    /// <summary>
    /// A wrapped publisher that can publish strings and bytes to RabbitMQ
    /// </summary>
    internal class RabbitMQPublisher : IRabbitMQPublisher
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly bool _persistent;
        private readonly string _exchange, _queue;
        private readonly bool _isDirectToQueue;

        internal RabbitMQPublisher(ConnectionFactory factory, string exchange, string queue, bool persistent, IConnection connection, IModel channel)
        {
            _persistent = persistent;
            _connection = connection;
            _channel = channel;
            _exchange = exchange ?? "";
            _queue = queue ?? "";
            _isDirectToQueue = (exchange == null);

            log.Debug($"Creating Publisher for {factory.UserName}@{_connection.Endpoint}");
        }

        public void Publish(string message, string messageId = null)
        {
            log.DebugFormat("Publishing to {0}, id={1}, message={2}", _isDirectToQueue ? _queue : _exchange, messageId, message);

            internalPublish(Encoding.UTF8.GetBytes(message), messageId);
        }

        public void Publish(byte[] message, string messageId = null)
        {
            log.DebugFormat("Publishing to {0}, id={1}, message=binary, length={2}", _isDirectToQueue ? _queue : _exchange, messageId, message.Length);

            internalPublish(message, messageId);
        }

        private void internalPublish(byte[] message, string messageId)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = _persistent;
            properties.MessageId = messageId ?? "";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(exchange: _exchange,
                                 routingKey: (_isDirectToQueue ? _queue : messageId),
                                 basicProperties: properties,
                                 body: message);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}