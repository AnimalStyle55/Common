namespace LD.Common.Queue
{
    /// <summary>
    /// Base rabbit mq settings
    /// </summary>
    public class RabbitMQSettings
    {
        /// <summary>Hostname of RabbitMQ Server</summary>
        public string Host { get; set; }
        /// <summary>Port for RabbitMQ Server</summary>
        public int Port { get; set; }
        /// <summary>Rabbit username</summary>
        public string Username { get; set; }
        /// <summary>Rabbit password</summary>
        public string Password { get; set; }
    }
}