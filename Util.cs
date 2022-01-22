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
        

        public static IEnumerable<JToken> AllTokens(JObject obj)
        {
            var toSearch = new Stack<JToken>(obj.Children());
            while (toSearch.Count > 0)
            {
                var inspected = toSearch.Pop();
                yield return inspected;
                foreach (var child in inspected)
                {
                    toSearch.Push(child);
                }
            }
        }
        

        public static T? QueryJsonValue<T>(string json, string query)
        {
            var queried = QueryJson(json, query);
            if(queried != null)
            {
                if(queried is JObject)
                {
                    var value = GetJsonValue<T>(queried);
                    return value;
                }
                else
                {
                    return default(T?);
                }
            }
            else
            {
                return default(T?);
            }
        }

        public static T? QueryJsonValue<T>(JToken token, string query)
        {
            var queried = QueryJson(token, query);
            if (queried != null)
            {
                if (queried is JObject || queried is JValue)
                {
                    var value = GetJsonValue<T>(queried);
                    return value;
                }
                else
                {
                    return default(T?);
                }
            }
            else
            {
                return default(T?);
            }
        }

        public static JToken? QueryJson(string json, string query)
        {
            var jsonData = (JToken?)JsonConvert.DeserializeObject(json);
            if(jsonData != null)
            {
                var queried = jsonData.SelectToken(query, false);
                return queried;
            }
            else
            {
                return null;
            }
        }

        public static JToken? QueryJson(JToken token, string query)
        {
            if (token != null)
            {
                var queried = token.SelectToken(query, false);
                return queried;
            }
            else
            {
                return null;
            }
        }

        public static T? GetJsonValue<T>(JToken token)
        {
            var output = token.Value<T>();
            return output != null ? output : default(T);
        }


    }

    
}
