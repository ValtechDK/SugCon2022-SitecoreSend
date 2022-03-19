using System.Linq;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace SugCon.SitecoreSend.ContentResolvers
{
    public class NewsletterContentResolver : RenderingContentsResolver
    {
        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var contextItem = GetContextItem(rendering, renderingConfig);
            return GetNewsletter(contextItem);
        }

        private object GetNewsletter(Item newsletterRoot)
        {
            var newsletterItem = newsletterRoot.Children.OrderBy(a => a["__sortorder"]).FirstOrDefault();

            var newsletter = new 
            {
                Title = newsletterItem["title"],
                Text = newsletterItem["text"],
                Articles = newsletterItem.Children.Select(x => new { Title = x["title"], Text = x["text"] })
            };
            return newsletter;
        }
    }
}