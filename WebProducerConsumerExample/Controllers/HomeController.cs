using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebProducerConsumerExample.Models;

namespace WebProducerConsumerExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _client = factory.CreateClient("1");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConsoleLog()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            //var range = Enumerable.Range(1, 1000);

            //var swx = Stopwatch.StartNew();

            //var tasks = range.Select(async i =>
            //{
            //    var sw = Stopwatch.StartNew();
            //    var response =
            //        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/barcode.png"));
            //    response.EnsureSuccessStatusCode();
            //    var content = await response.Content.ReadAsStringAsync();
            //    sw.Stop();
            //    _logger.LogInformation($"Content Size: {content.Length}. Time Taken: {sw.ElapsedMilliseconds}");
            //});

            //await Task.WhenAll(tasks).ConfigureAwait(false);
            //swx.Stop();
            //_logger.LogInformation($"Total Time Taken: {swx.ElapsedMilliseconds}");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
