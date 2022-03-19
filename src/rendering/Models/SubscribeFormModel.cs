using Sitecore.AspNet.RenderingEngine.Binding.Attributes;

namespace SugCon.SitecoreSend
{
    public class SubscribeFormModel
    {
        [SitecoreComponentField(Name = "List id")]
        public string ListId { get; set; }
        // public ICollection<MooSendCustomField> Fields { get; internal set; }
    }
}