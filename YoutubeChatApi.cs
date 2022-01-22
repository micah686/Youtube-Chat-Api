using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeChatApi.Enum;
using YoutubeChatApi.Models;
using System.Security.Cryptography;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.Reflection;

namespace YoutubeChatApi
{
    public class YoutubeChatApi
    {
        public static String userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36";

        private const String liveChatApi = "https://www.youtube.com/youtubei/v1/live_chat/get_live_chat?key=";
        private const String liveChatReplayApi = "https://www.youtube.com/youtubei/v1/live_chat/get_live_chat_replay?key=";
        private const String liveChatSendMessageApi = "https://www.youtube.com/youtubei/v1/live_chat/send_message?key=";


        private string videoId { get; set; }
        private string channelId { get; set; }
        private string continuation { get; set; }
        private bool isLiveReplay { get; set; }
        private bool isTopChatOnly { get; set; }
        private string visitorData { get; set; }
        //private ChatItem bannerItem { get; set; }
        //private List<ChatItem> chatItems { get; set; }
        //private List<ChatItem> chatItemTickerPaidMessages { get; set; }
        //private List<ChatItem> chatItemDeletes { get; set; }
        private string locale { get; set; }
        private string clientVersion { get; set; }
        private bool isInitialDataAvailable { get; set; }
        private string apiKey { get; set; }
        private string dataSyncId { get; set; }
        private int commentCounter { get; set; }
        private string clientMessageId { get; set; }
        private string parameters { get; set; }





        private static YoutubeCookies youtubeCookies;

        public YoutubeChatApi(string id)
        {
            GetInitialData(id, IdType.Video);
        }


        private void ParseActions(JArray json)
        {

            json.RemoveAt(0);
            foreach (var entry in json)
            {
                //var str = JsonConvert.SerializeObject(entry);
                //File.WriteAllText("output",str);
                var item = Util.GetJsonValue<JObject?>(entry, "replayChatItemAction.actions[0].addChatItemAction.item");
                if (item != null)
                {
                    var nextKey = item.Properties().Select(x => x.Name).FirstOrDefault();
                    if (nextKey != null)
                    {                      
                        switch (nextKey)
                        {
                            case "liveChatMembershipItemRenderer":
                                ParseChatItem(item, nextKey);
                                break;
                            case "addBannerToLiveChatCommand":
                                break;
                            case "liveChatBannerRenderer":
                                break;
                            case "addLiveChatTickerItemAction":
                                break;
                            case "markChatItemAsDeletedAction":
                                break;

                            default:
                                break;
                        }
                    }
                }


            }
        }

        private void ParseChatItem(JToken token, string ParentKey)
        {
            var chatItem = new ChatItem();
            if (ParentKey == "liveChatMembershipItemRenderer")
            {
                var jToken = Util.GetJsonValue<JToken?>(token, ParentKey);
                if (jToken != null)
                {
                    chatItem.AuthorName = Util.GetJsonValue(jToken, "authorName.simpleText");
                    chatItem.Id = Util.GetJsonValue(jToken, "id");


                }


            }
        }


        public void GetInitialData(string id, IdType type)
        {
            isInitialDataAvailable = true;
            var html = "";
            if(type == IdType.Video)
            {
                videoId = id;
                html = Util.GetPageContent($"https://www.youtube.com/watch?v={id}", GetHeader());
                var channelIdMatcher = Regex.Match(html, "\"channelId\":\"([^\"]*)\",\"isOwnerViewing\"");
                if(channelIdMatcher.Groups.Count > 0)
                {
                    channelId = channelIdMatcher.Groups[1].Value;
                }
                
            }
            else if(type == IdType.Channel)
            {
                channelId = id;
                html = Util.GetPageContent($"https://www.youtube.com/watch?v={id}/live", GetHeader());
                var videoIdMatcher = Regex.Match(html, "\"updatedMetadataEndpoint\":\\{\"videoId\":\"([^\"]*)");
                if(videoIdMatcher.Groups.Count > 0)
                {
                    videoId = videoIdMatcher.Groups[1].Value;
                }
                else
                {
                    throw new Exception($"The channel (ID:{channelId}) has not started live streaming!");
                }
            }

            var isLiveReplayMatcher = Regex.Match(html, "\"isReplay\":([^,]*)");
            if(isLiveReplayMatcher.Groups.Count > 0) { isLiveReplay = Convert.ToBoolean(isLiveReplayMatcher.Groups[1].Value); }

            var topOnlyContinuationMatcher = Regex.Match(html, "\"selected\":true,\"continuation\":\\{\"reloadContinuationData\":\\{\"continuation\":\"([^\"]*)");
                if (topOnlyContinuationMatcher.Groups.Count > 0) {continuation = topOnlyContinuationMatcher.Groups[1].Value;}

            if (!isTopChatOnly)
            {
                var allContinuationMatcher = Regex.Match(html, "\"selected\":false,\"continuation\":\\{\"reloadContinuationData\":\\{\"continuation\":\"([^\"]*)");
                if(allContinuationMatcher.Groups.Count > 0) { continuation = allContinuationMatcher.Groups[1].Value; }
            }

            var innertubeApiKeyMatcher = Regex.Match(html, "\"innertubeApiKey\":\"([^\"]*)\"");
            if(innertubeApiKeyMatcher.Groups.Count > 0) { apiKey = innertubeApiKeyMatcher.Groups[1].Value; }

            var datasyncIdMatcher = Regex.Match(html, "\"datasyncId\":\"([^|]*)\\|\\|.*\"");
            if (datasyncIdMatcher.Groups.Count > 0) { dataSyncId = datasyncIdMatcher.Groups[1].Value; }

            if (isLiveReplay)
            {
                html = Util.GetPageContent($"https://www.youtube.com/live_chat_replay?continuation={continuation}", new Dictionary<string, string>());
                var initJson = html.Substring(html.IndexOf("window[\"ytInitialData\"] = ") + "window[\"ytInitialData\"] = ".Length);
                initJson = initJson.Substring(0, initJson.IndexOf(";</script>"));
                var token = JToken.Parse(initJson);


                var continuationValue = Util.GetJsonValue(token, "continuationContents.liveChatContinuation.continuations[0].liveChatReplayContinuationData.continuation");
                if(continuationValue != null)
                {
                    continuation = continuationValue;
                }

                var actions = Util.GetJsonValue<JArray?>(token, "continuationContents.liveChatContinuation.actions");
                if (actions != null)
                {
                    ParseActions(actions);
                }


            }
            else
            {
                html = Util.GetPageContent($"https://www.youtube.com/live_chat?continuation={continuation}", GetHeader());
                var initJson = html.Substring(html.IndexOf("window[\"ytInitialData\"] = ") + "window[\"ytInitialData\"] = ".Length);
                initJson = initJson.Substring(0, initJson.IndexOf(";</script>"));
                //var json = Util.ToExpando(initJson);

                throw new NotImplementedException();
            }

        }

        public static string GetVideoIdFromUrl(string url)
        {
            string videoId = url;
            if(!videoId.Contains('?') && !videoId.Contains(".com/") && !videoId.Contains(".be/") &&
                !videoId.Contains('/') && !videoId.Contains('&'))       { return videoId; }
            string search = "";
            if (videoId.Contains("youtube.com/watch?v="))
            {
                search = "youtube.com/watch?v=";                                
            }
            else if (videoId.Contains("youtube.com/embed/"))
            {
                search = "youtube.com/embed/";
            }
            else if (videoId.Contains("youtu.be/"))
            {
                search = "youtu.be/";
            }

            string unsplitId = videoId.Substring(videoId.IndexOf(search) + search.Length);
            char[] invalidChars = "!@#$%^&*()/".ToCharArray();
            int pos = unsplitId.IndexOfAny(invalidChars, 0);
            videoId = unsplitId.Substring(0, pos);

            return videoId;
        }


        private Dictionary<string, string> GetHeader()
        {
            var header = new Dictionary<string, string>();
            if (!AllUserDataValid()) return header;
            var time = CurrentTimeMillis() / 1000;
            var origin = "https://www.youtube.com";
            var hash = $"{time} {youtubeCookies.SAPISID} {origin}";
            var ytc = youtubeCookies;
            var sha1 = SHA1.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(hash);
            byte[] sha1_result = sha1.ComputeHash(inputBytes);
            header.Add("Authorization", $"SAPISIDHASH {time}_{BitConverter.ToString(sha1_result).Replace("-", "")}");
            header.Add("X-Origin", origin);
            header.Add("Origin", origin);
            header.Add("Cookie", $"SAPISID={ytc.SAPISID}; HSID={ytc.HSID}; SSID={ytc.SSID}; APISID={ytc.APISID}; SID={ytc.SID}");
            return header;
        }




        /// <summary>
        /// Set the userData given a dictionary of Youtube Keys.
        /// </summary>
        /// <param name="userDataDict">Youtube Cookies keys. Should contain "SAPSID", "HSID", "SSID", "APISID", "SID"</param>
        /// <exception cref="ArgumentException"></exception>
        private void SetUserData(Dictionary<string, string> userDataDict)
        {
            if (userDataDict.Count == 5)//amount of total keys needed
            {
                string[] validKeys = { "SAPSID", "HSID", "SSID", "APISID", "SID" };
                var validDict = validKeys.All(k => userDataDict.ContainsKey(k));
                if (validDict == false)
                {
                    throw new ArgumentException("Invalid Keys. There should be: SAPSID, HSID, SSID, APISID, SID keys");
                }
                else
                {
                    var SAPSID = userDataDict.FirstOrDefault(x => x.Key == "SAPSID").Value;
                    var HSID = userDataDict.FirstOrDefault(x => x.Key == "HSID").Value;
                    var SSID = userDataDict.FirstOrDefault(x => x.Key == "SSID").Value;
                    var APISID = userDataDict.FirstOrDefault(x => x.Key == "APISID").Value;
                    var SID = userDataDict.FirstOrDefault(x => x.Key == "SID").Value;
                    SetUserData(SAPSID, HSID, SSID, APISID, SID);
                }
            }
            else
            {
                throw new ArgumentException("Invalid amount of keys There should only be 5 keys");
            }
        }

        /// <summary>
        /// Sets the appropriate youtube cookies
        /// </summary>
        /// <param name="SAPSID">Youtube Cookie</param>
        /// <param name="HSID">Youtube Cookie</param>
        /// <param name="SSID">Youtube Cookie</param>
        /// <param name="APISID"Youtube Cookie></param>
        /// <param name="SID">Youtube Cookie</param>
        private void SetUserData(string SAPSID, string HSID, string SSID, string APISID, string SID)
        {
            youtubeCookies = new YoutubeCookies(SAPSID, HSID, SSID, APISID, SID);
            if (!AllUserDataValid())
            {
                throw new ArgumentException("One or more of the IDs are invalid");
            }
        }

        /// <summary>
        /// Checks if all Youtube Cookie values are not empty
        /// </summary>
        /// <returns></returns>
        private bool AllUserDataValid()
        {
            var c1 = !string.IsNullOrEmpty(youtubeCookies.SAPISID);
            var c2 = !string.IsNullOrEmpty(youtubeCookies.HSID);
            var c3 = !string.IsNullOrEmpty(youtubeCookies.SSID);
            var c4 = !string.IsNullOrEmpty(youtubeCookies.APISID);
            var c5 = !string.IsNullOrEmpty(youtubeCookies.SID);
            return c1 && c2 && c3 && c4 && c5;
        }

        private static long CurrentTimeMillis()
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

    
}
