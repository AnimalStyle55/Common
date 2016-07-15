using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Queue.Subscriber
{
    /// <summary>
    /// Interface for a RabbitMQ Subscriber
    /// </summary>
    public interface IRabbitMQSubscriber : IDisposable
    {
        /// <summary>
        /// Stop the receivers from processing.  Waits for active receivers to end (up to optional timeout)
        /// NOTE: This does not actually disconnect anything, it is expected that .Dispose() would be called after this completes
        /// </summary>
        /// <param name="timeout">
        /// (optional) timeout for waiting for active receivers to stop.
        /// If waiting times out, those active messages will be requeued.
        /// If null, will wait up to forever
        /// </param>
        void Stop(TimeSpan? timeout = null);
    }
}