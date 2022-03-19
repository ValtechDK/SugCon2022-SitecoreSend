using Microsoft.AspNetCore.Mvc;
using SugCon.SitecoreSend.Models;
using SugCon.SitecoreSend.Services;
using System.Threading.Tasks;

namespace SugCon.SitecoreSend.Controllers
{
    [ApiController]
    public class SubscribeController : Controller
    {
        private readonly ISendService _service;

        public SubscribeController(ISendService service)
        {
            _service = service;
        }

        [Route("/subscribe/{listId}"), HttpPost]
        public async Task<IActionResult> Index(string listId, MooSendSubscriber subscriber)
        {
            await _service.Subscribe(listId, subscriber);
            return Redirect("/?subscribe=ok");
        }
    }

    public class SubscribeModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
