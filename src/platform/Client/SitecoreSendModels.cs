using System.Collections.Generic;

namespace SugCon.SitecoreSend.Models
{
    public class MooSendResponse<T>
    {
        public int Code { get; set; }
        public string Error { get; set; }
        public T Context { get; set; }
    }

    public class MooSendListContext
    {
        public ICollection<MooSendList> MailingLists { get; set; }
    }

    public class MooSendList
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
