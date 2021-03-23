using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProducerConsumerExample.HostedServices;
using WebProducerConsumerExample.Models;

namespace WebProducerConsumerExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueuesController : ControllerBase
    {
        private readonly IMessageProducer _producer;

        public QueuesController(IMessageProducer producer)
        {
            _producer = producer;
        }

        [HttpPost]
        [Route("ControlMessage")]
        public async Task<IActionResult> SendControlMessage(ControlMessage message)
        {
            await _producer.SendControlMessage(message);
            return Ok();
        }

        [HttpPost]
        [Route("WorkMessage")]
        public async Task<IActionResult> SendWorkMessage(FileDownloadAction model)
        {
            var batchStart = DateTime.UtcNow;
            //for (int i = 0; i < model.MessageCount; i++)
            //{
            //    await _producer.SendWorkMessage(
            //        new WorkMessage
            //        {
            //            Started = batchStart,
            //            Url = "http://localhost/barcode.png",
            //            LastInBatch = i == model.MessageCount
            //        });
            //}

            var tasks = Enumerable.Range(1, model.MessageCount).Select(async i => await _producer.SendWorkMessage(
                new WorkMessage
                {
                    Started = batchStart,
                    Url = "http://localhost/barcode.png",
                    LastInBatch = i == model.MessageCount
                }));

            Task.WaitAll(tasks.ToArray());

            return Ok();
        }
    }
}
