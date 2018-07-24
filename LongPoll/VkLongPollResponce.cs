using Newtonsoft.Json.Linq;

namespace WaterStrike.LongPoll
{
    public class VkLongPollResponce
    {
        public int Failed { get; set; }
        public ulong Ts { get; set; }
        public VkUpdate[] Updates { get; set; }
    }
}
