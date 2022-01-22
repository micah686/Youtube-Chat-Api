using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeChatApi
{
    public static class Util
    {

        


        public static string GetPageContent(string url, Dictionary<string, string> headers)
        {
            var client = new HttpClient();

            AddRequestHeaders(ref headers);

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            client.DefaultRequestHeaders.UserAgent.ParseAdd(YoutubeChatApi.userAgent);

            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                    var html = content.ReadAsStringAsync().Result;
                    return html;
                }
            }

        }


        public static void AddRequestHeaders(ref Dictionary<string, string> headers)
        {
            headers.Add("Accept-Charset", "utf-8");
            headers.Add("User-Agent", YoutubeChatApi.userAgent);
        }
        

        


        
        

        public static JToken? GetJToken(JToken token, string query)
        {
            var data = token.SelectToken(query, false);
            return data;
        }

        public static string GetJsonValue(JToken token, string query = "")
        {
            if(query != "")
            {
                var data = GetJToken(token, query);
                if(data != null)
                {
                    return data.Value<string>() ?? "";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return token.Value<string>() ?? "";
            }
        }

        public static T? GetJsonValue<T>(JToken token, string query = "")
        {
            if (query != "")
            {
                var data = GetJToken(token, query);
                if (data != null)
                {
                    return data.Value<T>() ?? default;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return token.Value<T>() ?? default;
            }
        }

        
    }

    
}
