using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebProducerConsumerExample.HostedServices
{
    public class BlockingQueueMessageProducer : IMessageProducer
    {
        private readonly BlockingCollection<ControlMessage> _controlQueue;
        private readonly BlockingCollection<WorkMessage> _workQueue;

        public BlockingQueueMessageProducer(BlockingCollection<ControlMessage> controlQueue, BlockingCollection<WorkMessage> workQueue)
        {
            _controlQueue = controlQueue;
            _workQueue = workQueue;
        }

        public async Task SendControlMessage(ControlMessage message)
        {
            _controlQueue.Add(message);
            await Task.CompletedTask;
        }

        public async Task SendWorkMessage(WorkMessage message)
        {
            _workQueue.Add(message);
            await Task.CompletedTask;
        }
    }
}