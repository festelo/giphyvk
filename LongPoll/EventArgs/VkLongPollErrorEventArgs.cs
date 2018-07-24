using System;
using System.Collections.Generic;
using System.Text;

namespace WaterStrike.LongPoll.EventArgs
{
    public class VkLongPollErrorEventArgs : VkLongPollEventArgs
    {
        public Exception InnerException { get; set; }
    }
}
