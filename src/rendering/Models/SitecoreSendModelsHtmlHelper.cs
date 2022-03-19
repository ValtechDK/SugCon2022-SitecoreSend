using System.Text.RegularExpressions;

namespace SugCon.SitecoreSend.Models
{
    public static class SitecoreSendModelsHtmlHelper
    {
        public static readonly Regex SafeNameReplacer = new Regex(@"[^a-z0-9_\-]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string HtmlInputType(this MooSendFieldType fieldType)
        {
            return fieldType.ToString().ToLower();
        }

        public static string HtmlSafeName(this MooSendCustomField field)
        {
            return SafeNameReplacer.Replace(field.Name, "__");
        }
    }


}
