using System.Collections.Generic;
using Sitecore.AspNet.RenderingEngine.Binding.Attributes;

namespace SugCon.SitecoreSend.Models
{
      public class NewsletterModel
    {
        [SitecoreComponentField] 
        public string Title { get; set; }

        [SitecoreComponentField] 
        public string Text { get; set; }

        [SitecoreComponentField]
        public IEnumerable<ArticleModel> Articles { get; set; }
    }
}
