using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SugCon.SitecoreSend.Models
{
    public class MooSendResponse<T>
    {
        public int Code { get;set; }
        public string Error { get; set; }
        public T Context { get; set; }
    }

    public class MooSendListDetails
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public ICollection<MooSendCustomField> CustomFieldsDefinition { get; set; }
    }

    public class MooSendCustomField
    {
        public string Name { get; set; }
        public MooSendFieldType Type { get; set; }
        public bool IsRequired { get; set; }
    }

    public enum MooSendFieldType
    {
        Text = 0,
        Number = 1,
        DateTime = 2,
        SingleDropDown = 3,
        Checkbox = 5,
    }

    public class MooSendSubscriber
    {
        public string Name { get; set;}
        public string Mobile { get; set; }
        public string Email { get; set; }
        
        [JsonIgnore]
        public IEnumerable<KeyValuePair<string,string>> CustomFieldsCollection { get; set; } = new List<KeyValuePair<string,string>>();

        [JsonProperty]
        public string[] CustomFields => CustomFieldsCollection?
            .Select(pair => $"{pair.Key}={pair.Value}")
            .ToArray();
    }
    
}
