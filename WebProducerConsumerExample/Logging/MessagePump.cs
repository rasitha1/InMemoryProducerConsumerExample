using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebProducerConsumerExample.Logging
{
    public class MessagePump
    {
        private readonly Task _task;
        private readonly Queue<LogMessage> _buffer;
        private readonly IHubContext<LogHub, ILogHub> _hubContext;
        private const int Max = 200;

        public MessagePump(Queue<LogMessage> buffer, IHubContext<LogHub, ILogHub> hubContext)
        {
            _buffer = buffer;
            _hubContext = hubContext;
            _task = Task.Factory.StartNew(ProcessMessages, TaskCreationOptions.LongRunning);
        }
        private void ProcessMessages()
        {
            while (true)
            {
                var message = LogQueue.Queue.Take();
                _buffer.Enqueue(message);
                if (_buffer.Count > Max)
                {
                    _buffer.Dequeue();
                }
                _hubContext.Clients.All.WriteLogMessage(message);
            }
        }

        public Queue<LogMessage> GetBuffer() => _buffer;

    }
}