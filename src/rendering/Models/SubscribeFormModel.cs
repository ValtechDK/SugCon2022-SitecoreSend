using Sitecore.LayoutService.Client.Response.Model;
using Sitecore.LayoutService.Client.Response.Model.Fields;
using SugCon.SitecoreSend.Models;
using System.Collections.Generic;

namespace SugCon.SitecoreSend
{
    public class SubscribeFormModel
    {
        public TextField ListId { get; set; }
        
        public ICollection<MooSendCustomField> Fields { get; internal set; }

        public Route Route { get; set; }
    }
}