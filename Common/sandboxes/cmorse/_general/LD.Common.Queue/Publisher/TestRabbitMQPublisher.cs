using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Queue.Publisher
{
    /// <summary>
    /// Test rabbitmq publisher that only logs
    /// </summary>
    internal class TestRabbitMQPublisher : IRabbitMQPublisher
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        private readonly string _queueName;
        private readonly string _exchangeName;

        public TestRabbitMQPublisher(string queueName, string exchangeName)
        {
            _queueName = queueName;
            _exchangeName = exchangeName;

            log.Debug($"Creating Test Publisher for {destName()}");
        }

        public void Publish(byte[] message, string messageId = null)
        {
            log.DebugFormat("(Test) Publishing to {0}, id={1}, message=binary, length={2}", destName(), messageId, message.Length);
        }

        public void Publish(string message, string messageId = null)
        {
            log.DebugFormat("(Test) Publishing to {0}, id={1}, message={2}", destName(), messageId, message);
        }

        private string destName()
        {
            return _queueName ?? _exchangeName;
        }

        public void Dispose()
        {
            // Nothing to dispose for this implementation.
        }
    }
}