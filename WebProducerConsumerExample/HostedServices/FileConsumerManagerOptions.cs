namespace WebProducerConsumerExample.HostedServices
{
    public class FileConsumerManagerOptions
    {
        public int DefaultConsumerCount { get; set; } = 2;
        public bool StartConsumersAtStartup { get; set; } = true;
    }
}