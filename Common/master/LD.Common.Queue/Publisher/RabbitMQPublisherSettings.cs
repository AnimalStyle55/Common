using System;

namespace LD.Common.Queue.Publisher
{
    /// <summary>
    /// Settings specific for subscribers
    /// </summary>
    public class RabbitMQPublisherSettings : LD.Common.Queue.RabbitMQSettings
    {
        /// <summary>disable waiting for confirms</summary>
        public bool DisableConfirms { get; set; }
        /// <summary>how long to wait for confirms</summary>
        public TimeSpan? ConfirmsTimeout { get; set; }
    }
}