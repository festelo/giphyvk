using System;
using System.Collections.Generic;
using System.Text;

namespace WaterStrike.LongPoll.EventArgs
{
    public class VkLongPollUpdateEventArgs : VkLongPollEventArgs
    {
        public VkUpdate Update { get; set; }
    }
}
