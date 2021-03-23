using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebProducerConsumerExample.Logging
{
    public class LogHub : Hub<ILogHub>
    {

        private readonly MessagePump _pump;

        public LogHub(MessagePump pump)
        {
            _pump = pump;
        }



        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.WriteBatch(_pump.GetBuffer());
        }
    }
}