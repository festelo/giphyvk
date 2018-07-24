using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WaterStrike.LongPoll.EventArgs;

namespace WaterStrike.LongPoll
{
    public class VkLongPollClient : IDisposable
    {
        private readonly HttpClient client = new HttpClient();
        public VkLongPollClient()
        {
        }

        private CancellationTokenSource tokenSource;
        private CancellationToken token => tokenSource.Token;
        public bool IsListening => tokenSource != null;

        public event EventHandler<VkLongPollErrorEventArgs> Error;
        public event EventHandler<VkLongPollUpdateEventArgs> Message;

        public async Task StartListener(string key, string server, ulong ts)
        {
            if (tokenSource != null)
            {
                throw new InvalidOperationException("Listening already started. You should to stop it before starting again");
            }
            tokenSource = new CancellationTokenSource();
            await Task.Factory.StartNew(async () =>
            {
                var curts = ts;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var resp = await GetResponce(key, server, curts, token);
                        if (resp.Failed != 0)
                        {
                            switch (resp.Failed)
                            {
                                case 2:
                                    throw new VkLongPollException("Long poll data outdated");
                                case 3:
                                    throw new VkLongPollException("Long poll data outdated");
                                default:
                                    break;
                            }
                        }
                        curts = resp.Ts;
                        foreach (var update in resp.Updates)
                        {
                            Message?.Invoke(this, new VkLongPollUpdateEventArgs() { Update = update });
                        }
                    }
                    catch(TaskCanceledException) { }
                    catch (Exception e)
                    {
                        Error?.Invoke(this, new VkLongPollErrorEventArgs() {InnerException = e});
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private async Task<VkLongPollResponce> GetResponce(string key, string server, ulong ts, CancellationToken token)
        {
            var url = $"{server}?act=a_check&key={key}&ts={ts}&wait=25&mode=2&version=2";
            var res = await client.GetAsync(url, token);
            var str = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VkLongPollResponce>(str);
        }

        public void Stop()
        {
            tokenSource.Cancel();
            tokenSource = null;
        }

        public void Dispose()
        {
            client?.Dispose();
            tokenSource?.Dispose();
        }
    }
}
