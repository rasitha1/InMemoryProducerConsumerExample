using System;

namespace WebProducerConsumerExample.HostedServices
{
    public class WorkMessage
    {
        public string Url { get; set; }
        public DateTime Started { get; set; }
        public bool LastInBatch { get; set; }
    }
}