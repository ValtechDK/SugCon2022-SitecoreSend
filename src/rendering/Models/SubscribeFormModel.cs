using Sitecore.LayoutService.Client.Response.Model;
using Sitecore.LayoutService.Client.Response.Model.Fields;
using SugCon.SitecoreSend.Models;
using System.Collections.Generic;

namespace SugCon.SitecoreSend
{
    public class SubscribeFormModel
    {
        public ItemLinkField ListId { get; set; }
        
        public ICollection<MooSendCustomField> Fields { get; internal set; } = new List<MooSendCustomField>(0);

        public Route Route { get; set; }

        public string ListIdString => ListId?.Id.ToString();
    }
}