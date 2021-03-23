namespace WebProducerConsumerExample.HostedServices
{
    public class ControlMessage
    {
        public int ConsumerCount { get; set; }
        public bool LogStats { get; set; }

        public override string ToString()
        {
            return $"ConsumerCount={ConsumerCount}";
        }
    }
}