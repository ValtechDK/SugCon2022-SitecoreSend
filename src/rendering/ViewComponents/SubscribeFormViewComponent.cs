using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNet.RenderingEngine.Binding;
using SugCon.SitecoreSend.Services;
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
            var model = await _binder.Bind<SubscribeFormModel>(ViewContext);
            var fields = await _sendService.GetListCustomFields(model.ListId);
            // model.Fields = fields;
            return View("SubscribeForm", model);
        }
    }
}