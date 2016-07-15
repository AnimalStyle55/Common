namespace LD.Common.Queue.Publisher
{
    /// <summary>
    /// Settings specific for subscribers
    /// </summary>
    public class RabbitMQPublisherSettings : LD.Common.Queue.RabbitMQSettings
    {
        /// <summary>Queue Name to subscribe</summary>
        public string QueueName { get; set; }
        /// <summary>How many connections for subscription</summary>
        public int Connections { get; set; }
        /// <summary>How many channels per connection</summary>
        public int ChannelsPerConnection { get; set; }
    }
}