using System;
using System.Text;
using System.Threading;

namespace GiphyVk
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var group = ulong.Parse(Environment.GetEnvironmentVariable("group"));
            var key = Environment.GetEnvironmentVariable("key");
            var gkey = Environment.GetEnvironmentVariable("giphyKey");
            var controller = new VkController(group, key, gkey);
            controller.StartLongPoll().Wait();
            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
