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
    public GiphyApi(string token) {
        this.token = token;
    }
    public async Task<string> Random(string query){
        var url = string.Format("https://api.giphy.com/v1/gifs/random?api_key={0}&tag={1}&rating=R", 
            token, 
            HttpUtility.UrlEncode(query));
        var resp = await client.GetStringAsync(url);
        var jobj = JsonConvert.DeserializeObject<dynamic>(resp);
        var list = jobj?.data as IEnumerable<dynamic>;
        var gifdata = list?.ElementAtOrDefault(0);
        if(gifdata == null) {
            throw new Exception("Not found");
        }
        return gifdata.images.downsized.url;
    }
    public async Task<string> Search(string query, int offset = 0, string lang = "en"){
        var url = string.Format("https://api.giphy.com/v1/gifs/search?api_key={0}&q={1}&limit=1&offset={2}}&rating=R&lang={3}", 
            token, 
            HttpUtility.UrlEncode(query),
            offset,
            lang);
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