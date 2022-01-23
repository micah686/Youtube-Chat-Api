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





        #region "Json/JToken Queries"

        /// <summary>
        /// Gets a particular JToken
        /// </summary>
        /// <param name="token">The root JToken to search</param>
        /// <param name="query">The search query to search in the token</param>
        /// <returns>Returns the JToken that matches the query, or null if invalid</returns>
        public static JToken? GetJToken(JToken token, string query)
        {
            if(token != null)
            {
                var data = token.SelectToken(query, false);
                return data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a particular JToken
        /// </summary>
        /// <param name="json">The json to search</param>
        /// <param name="query">The search query to search in the token</param>
        /// <returns>Returns the JToken that matches the query, or null if invalid</returns>
        public static JToken? GetJToken(string json, string query)
        {
            var token = JToken.Parse(json);
            if(token != null)
            {
                var data = GetJToken(token, query);
                return data;
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// Gets the string value from a JToken given a query
        /// </summary>
        /// <param name="token">The root JToken to search</param>
        /// <param name="query">The query to search in the JToken</param>
        /// <returns>Returns the string value from the query in the JToken, or an empty string if invalid</returns>
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

        /// <summary>
        /// Gets the specified object Type from a JToken given a query
        /// </summary>
        /// <typeparam name="T">Type you want returned</typeparam>
        /// <param name="token">The root JToken to search</param>
        /// <param name="query">The query to search in the JToken</param>
        /// <returns>Retunrs the value as the specified Type</returns>
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
        #endregion

    }


}
