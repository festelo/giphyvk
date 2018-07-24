using System;
using System.Collections.Generic;
using System.Text;

namespace GiphyVk.LongPoll.EventArgs
{
    public class VkLongPollUpdateEventArgs : VkLongPollEventArgs
    {
        public VkUpdate Update { get; set; }
    }
}
