using Microsoft.AspNetCore.Mvc;
using SugCon.SitecoreSend.Models;
using SugCon.SitecoreSend.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SugCon.SitecoreSend.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly ISendService _service;

        public SubscribeController(ISendService service)
        {
            _service = service;
        }

        [Route("/subscribe/{listId}"), HttpPost]
        public async Task<IActionResult> Index(string listId, MooSendSubscriberUpdate subscriber)
        {
            var fields = await _service.GetListCustomFields(listId);
            subscriber.CustomFields = fields.Select(field => new
            {
                field,
                postedValue = Request.Form[field.HtmlSafeName()],
            }).Where(x => !string.IsNullOrEmpty(x.postedValue))
            .Select(x =>
            {
                var value = x.postedValue;
                if(x.field.Type == MooSendFieldType.Checkbox)
                {
                    value = "true";
                }
                return $"{x.field.Name}={value}";
            })
            .ToArray();

            try
            {
                await _service.Subscribe(listId, subscriber);
                return Redirect("/?subscribe=ok");
            }
            catch (Exception exc)
            {
                return Redirect($"/?subscribe=error&error={exc.Message})");
            }
        }
    }

    public class SubscribeModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
