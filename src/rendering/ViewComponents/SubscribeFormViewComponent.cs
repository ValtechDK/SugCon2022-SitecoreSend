using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNet.RenderingEngine.Binding;
using SugCon.SitecoreSend.Services;
using System;
using System.Threading.Tasks;

namespace SugCon.SitecoreSend
{
    public class SubscribeFormViewComponent : ViewComponent
    {
        private readonly IViewModelBinder _binder;
        private readonly ISendService _sendService;

        public SubscribeFormViewComponent(IViewModelBinder binder, ISendService sendService)
        {
            _binder = binder;
            _sendService = sendService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            if(ViewContext.HttpContext.Request.Query["subscribe"] == "ok")
            {
                return View("Confirmed");
            }

            var model = await _binder.Bind<SubscribeFormModel>(ViewContext);
            if (model.ListId != null && model.ListId.Id != Guid.Empty)
            {
                var fields = await _sendService.GetListCustomFields(model.ListId.Id);
                model.Fields = fields;
            }

            return View("SubscribeForm", model);
        }
    }
}