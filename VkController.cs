using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using GiphyVk.LongPoll;
using GiphyVk.LongPoll.EventArgs;

namespace GiphyVk
{
    class VkController : IDisposable
    {
        private readonly VkApi api = new VkApi();
        private readonly ulong groupid;        
        private readonly GiphyApi gapi;
        private readonly VkLongPollClient longPollClient = new VkLongPollClient(); 
        public string DefaultLanguage { get; set; }= "en";
        public VkController(ulong groupid, string apptoken, string gtoken)
        {
            this.groupid = groupid;
            this.gapi = new GiphyApi(gtoken);
            api.Authorize(new ApiAuthParams() {AccessToken = apptoken});
            longPollClient.Message += MessageReceived;
            longPollClient.Error += ErrorReceived;
        }

        private async void ErrorReceived(object sender, VkLongPollErrorEventArgs e)
        {
            if (e.InnerException is VkLongPollException)
            {
                await StartLongPoll();
            }
        }

        public async Task StartLongPoll()
        {
            if (longPollClient.IsListening)
            {
                longPollClient.Stop();
            }
            var longpoll = await api.Groups.GetLongPollServerAsync(groupid);
            await longPollClient.StartListener(longpoll.Key, longpoll.Server, longpoll.Ts);
        }
        
        private async void MessageReceived(object sender, VkLongPollUpdateEventArgs e)
        {
            if(e.Update.Type != "message_new") return;

            var obj = e.Update.Object;
            var id = (long)obj["user_id"];
            var message = obj.ToObject<Message>();

            if(string.IsNullOrWhiteSpace(message.Body)) return;
            
            var searchRegex = new Regex("([^#]+)#?(\\d+)?");
            var randomRegex = new Regex("([^#]+)#(?:rnd|random|r)");
            Task<string> getUrl = null;
              
            if(randomRegex.Match(message.Body) is var rndMatch && rndMatch.Success) {
                var query = rndMatch.Groups[1].Value;
                getUrl = gapi.Random(query);
            }  
            else
            
            if(searchRegex.Match(message.Body) is var searchMatch && searchMatch.Success) {
                var query = searchMatch.Groups[1].Value;
                var offset = !string.IsNullOrWhiteSpace(searchMatch.Groups[2].Value) 
                    ? int.Parse(searchMatch.Groups[2].Value) 
                    : 0;
                getUrl = gapi.Search(query, offset, DefaultLanguage);
            }
            else return;

            try
            {
                var data = await api.Docs.GetMessagesUploadServerAsync(id, DocMessageType.Doc);
                var url = await getUrl;
                var bytes = await Download(url);
                var response = await UploadFile(data.UploadUrl, bytes);
                var docs = api.Docs.Save(response, "gif");
                api.Messages.Send(new MessagesSendParams
                {
                    Attachments = docs,
                    UserId = id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                api.Messages.Send(new MessagesSendParams
                {
                    Message = "Увы",
                    UserId = id
                });
            }
        }

        private async Task<byte[]> Download(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetByteArrayAsync(url);
            }
        }
        public static async Task<string> UploadFile(string url, byte[] data) 
        { 
            using (var client = new HttpClient()) 
            {
                var requestContent = new MultipartFormDataContent();
                var documentContent= new ByteArrayContent(data);
                documentContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                requestContent.Add(documentContent, "file", "image.gif");

                var response = await client.PostAsync(url, requestContent);

                return Encoding.ASCII.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }

        public void Dispose()
        {
            longPollClient?.Dispose();
            api?.Dispose();
            gapi?.Dispose();
        }
    }
}