using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Queue.Publisher
{
    /// <summary>
    /// Interface for a RabbitMQ Publisher
    /// </summary>
    public interface IRabbitMQPublisher : IDisposable
    {
        /// <summary>
        /// Publish a string message
        /// </summary>
        /// <param name="message">will be encoded as UTF-8</param>
        /// <param name="messageId">an optional (but recommended) message id</param>
        /// <exception cref="System.IO.IOException">thrown if publish is not confirmed (if confirms enabled)</exception>
        void Publish(string message, string messageId = null);

        /// <summary>
        /// Publish a byte[] message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageId">an optional (but recommended) message id, also used for routing messages if direct or topic exchanges are used</param>
        /// <exception cref="System.IO.IOException">thrown if publish is not confirmed (if confirms enabled)</exception>
        void Publish(byte[] message, string messageId = null);
    }
}