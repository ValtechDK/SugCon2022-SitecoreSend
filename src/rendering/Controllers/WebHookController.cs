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
        public ContentResult WebHook([FromBody] object obj)
        {
            _logger.LogInformation($"Received webhook: \r\n{JsonConvert.SerializeObject(obj, Formatting.Indented)}");
            return Content("OK");
        }
    }
}
