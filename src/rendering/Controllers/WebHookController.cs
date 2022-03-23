using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SugCon.SitecoreSend.Controllers
{
    public class WebHookController : Controller
    {
        private readonly ILogger<WebHookController> _logger;

        public WebHookController(ILogger<WebHookController> logger)
        {
            _logger = logger;
        }

        [Route("/api/webhooks/incoming/moosend"), HttpPost]
        public ContentResult WebHookPost([FromBody] object obj)
        {
            _logger.LogInformation($"Received webhook POST: \r\n{JsonConvert.SerializeObject(obj, Formatting.Indented)}");
            return Content("OK");
        }

        [Route("/api/webhooks/incoming/moosend"), HttpGet]
        public ContentResult WebHookGet([FromQuery] object obj)
        {
            _logger.LogInformation($"Received webhook GET: \r\n{JsonConvert.SerializeObject(obj, Formatting.Indented)}");
            return Content("OK");
        }

        [Route("/api/webhooks/incoming/moosend"), HttpHead]
        public ContentResult WebHookHead([FromQuery] object obj)
        {
            _logger.LogInformation($"Received webhook HEAD: \r\n{JsonConvert.SerializeObject(obj, Formatting.Indented)}");
            return Content("OK");
        }
    }
}
