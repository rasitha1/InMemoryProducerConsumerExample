using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebProducerConsumerExample.Logging
{
    public interface ILogHub
    {
        Task WriteLogMessage(LogMessage data);
        Task WriteBatch(Queue<LogMessage> buffer);
    }
}