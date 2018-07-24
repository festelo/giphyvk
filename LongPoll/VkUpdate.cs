using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WaterStrike.LongPoll
{
    public class VkUpdate
    {
        public string Type { get; set; }
        public JToken Object { get; set; }
        public ulong GroupId { get; set; }
    }
}
