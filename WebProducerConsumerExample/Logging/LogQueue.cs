using System.Collections.Concurrent;
using System.Linq;

namespace WebProducerConsumerExample.Logging
{
    public static class LogQueue
    {
        public static BlockingCollection<LogMessage> Queue = new BlockingCollection<LogMessage>();
    }
}
