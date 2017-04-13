using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCoach.App.FacebookEntities
{
    public class Rootobject
    {
        public string _object { get; set; }
        public Entry[] entry { get; set; }
    }

    public class Entry
    {
        public string id { get; set; }
        public long time { get; set; }
        public Messaging[] messaging { get; set; }
    }

    public class Messaging
    {
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public long timestamp { get; set; }
        public Message message { get; set; }
    }

    public class Sender
    {
        public string id { get; set; }
    }

    public class Recipient
    {
        public string id { get; set; }
    }

    public class Message
    {
        public string mid { get; set; }
        public int seq { get; set; }
        public string text { get; set; }
        public Attachment[] attachments { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public string url { get; set; }
    }

}
