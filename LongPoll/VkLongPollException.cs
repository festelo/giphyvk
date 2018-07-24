using System;

namespace WaterStrike.LongPoll
{
    public class VkLongPollException : Exception
    {
        public VkLongPollException(string message) : base(message) { }
    }
}
