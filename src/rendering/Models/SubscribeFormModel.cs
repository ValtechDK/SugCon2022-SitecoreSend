using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace SugCon.SitecoreSend
{
    public class SubscribeFormModel
    {
        [SitecoreComponentField]
        public TextField ListId { get; set; }
        // public ICollection<MooSendCustomField> Fields { get; internal set; }
    }
}