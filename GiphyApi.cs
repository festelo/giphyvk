using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

class GiphyApi : IDisposable {
    private HttpClient client = new HttpClient();
    private string token;
    public string Region { get; set; } = "en";
    public GiphyApi(string token) {
        this.token = token;
    }
    public async Task<string> Search(string query){
        var url = string.Format("https://api.giphy.com/v1/gifs/search?api_key={0}&q={1}&limit=1&offset=0&rating=G&lang={2}", 
            token, 
            HttpUtility.UrlEncode(query),
            Region);
        var resp = await client.GetStringAsync(url);
        var jobj = JsonConvert.DeserializeObject<dynamic>(resp);
        var list = jobj?.data as IEnumerable<dynamic>;
        var gifdata = list?.ElementAtOrDefault(0);
        if(gifdata == null) {
            throw new Exception("Not found");
        }
        return gifdata.images.downsized.url;
    }

    public void Dispose()
    {
        client.Dispose();
    }
}