using System;

namespace LD.Common.Queue.Subscriber
{
    /// <summary>
    /// Settings specific for subscribers
    /// </summary>
    public class RabbitMQSubscriberSettings : LD.Common.Queue.RabbitMQSettings
    {
        /// <summary>Queue Name to subscribe</summary>
        public string QueueName { get; set; }

        /// <summary>How many connections for subscription</summary>
        public int Connections { get; set; }

        /// <summary>How many channels per connection</summary>
        public int ChannelsPerConnection { get; set; }

        /// <summary>How long to wait for confirmation of a republish (also blocks acknowledgement)</summary>
        public TimeSpan? RepublishConfirmsTimeout { get; set; }

        /// <summary>Disable confirms on republish (dangerous, acks might succeed while repub fails)</summary>
        public bool DisableRepublishConfirms { get; set; }
    }
}