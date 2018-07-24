using System;

namespace GiphyVk.LongPoll
{
    public class VkLongPollException : Exception
    {
        public VkLongPollException(string message) : base(message) { }
    }
}
