using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCoach.App.SlackEntities
{

    public class Rootobject
    {
        public string token { get; set; }
        public string team_id { get; set; }
        public string api_app_id { get; set; }
        public Event _event { get; set; }
        public string type { get; set; }
        public string[] authed_users { get; set; }
        public string event_id { get; set; }
        public int event_time { get; set; }
    }

    public class Event
    {
        public string type { get; set; }
        public string subtype { get; set; }
        public string text { get; set; }
        public File file { get; set; }
        public string user { get; set; }
        public bool upload { get; set; }
        public bool display_as_bot { get; set; }
        public string username { get; set; }
        public object bot_id { get; set; }
        public string ts { get; set; }
        public string channel { get; set; }
        public string event_ts { get; set; }
    }

    public class File
    {
        public string id { get; set; }
        public int created { get; set; }
        public int timestamp { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string mimetype { get; set; }
        public string filetype { get; set; }
        public string pretty_type { get; set; }
        public string user { get; set; }
        public bool editable { get; set; }
        public int size { get; set; }
        public string mode { get; set; }
        public bool is_external { get; set; }
        public string external_type { get; set; }
        public bool is_public { get; set; }
        public bool public_url_shared { get; set; }
        public bool display_as_bot { get; set; }
        public string username { get; set; }
        public string url_private { get; set; }
        public string url_private_download { get; set; }
        public string thumb_64 { get; set; }
        public string thumb_80 { get; set; }
        public string thumb_360 { get; set; }
        public int thumb_360_w { get; set; }
        public int thumb_360_h { get; set; }
        public string thumb_480 { get; set; }
        public int thumb_480_w { get; set; }
        public int thumb_480_h { get; set; }
        public string thumb_160 { get; set; }
        public string thumb_720 { get; set; }
        public int thumb_720_w { get; set; }
        public int thumb_720_h { get; set; }
        public string thumb_800 { get; set; }
        public int thumb_800_w { get; set; }
        public int thumb_800_h { get; set; }
        public string thumb_960 { get; set; }
        public int thumb_960_w { get; set; }
        public int thumb_960_h { get; set; }
        public string thumb_1024 { get; set; }
        public int thumb_1024_w { get; set; }
        public int thumb_1024_h { get; set; }
        public int image_exif_rotation { get; set; }
        public int original_w { get; set; }
        public int original_h { get; set; }
        public string permalink { get; set; }
        public string permalink_public { get; set; }
        public object[] channels { get; set; }
        public object[] groups { get; set; }
        public object[] ims { get; set; }
        public int comments_count { get; set; }
    }


}
