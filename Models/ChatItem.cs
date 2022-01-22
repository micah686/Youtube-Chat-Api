using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChatApi.Enum;

namespace YoutubeChatApi.Models
{
    public class ChatItem
    {
        public ChatItemType Type { get; internal set; }
        public string AuthorName { get; internal set; }
        public string AuthorChannelID { get; internal set; }
        public string Message { get; internal set; }
        public List<object> MessageExtended { get; internal set; }
        public string AuthorIconUrl { get; internal set; }
        public string Id { get; internal set; }
        public long Timestamp { get; internal set; }
        public List<AuthorType> AuthorType { get; internal set; }
        public string MemberBadgeIconUrl { get; internal set; }
        //For paid Message
        public int BodyBackgroundColor { get; internal set; }
        public int BodyTextColor { get; internal set; }
        public int HeaderBackgroundColor { get; internal set; }
        public int HeaderTextColor { get; internal set; }
        public int AuthorNameColor { get; internal set; }
        public string PurchaseAmount { get; internal set; }
        //For paid sticker
        public string StickerIconUrl { get; internal set; }
        public int BackgroundColor { get; internal set; }
        //For ticker paid messages
        public int EndBackgroundColor { get; internal set; }
        public int DurationSec { get; internal set; }
        public int FullDurationSec { get; internal set; }



        internal ChatItem()
        {
            Type = ChatItemType.Message;
            AuthorType = new List<AuthorType>();
            AuthorType.Add(Enum.AuthorType.Normal);
        }

        public bool IsAuthorVerified()
        {
            return AuthorType.Contains(Enum.AuthorType.Verified);
        }

        public bool IsAuthorOwner()
        {
            return AuthorType.Contains(Enum.AuthorType.Owner);
        }

        public bool IsAuthorModerator()
        {
            return AuthorType.Contains(Enum.AuthorType.Moderator);
        }

        public bool IsAuthorMember()
        {
            return AuthorType.Contains(Enum.AuthorType.Member);
        }

        public override string ToString()
        {
            return "ChatItem{" +
                "type=" + Type +
                ", authorName='" + AuthorName + '\'' +
                ", authorChannelID='" + AuthorChannelID + '\'' +
                ", message='" + Message + '\'' +
                ", messageExtended=" + MessageExtended +
                ", iconURL='" + AuthorIconUrl + '\'' +
                ", id='" + Id + '\'' +
                ", timestamp=" + Timestamp +
                ", authorType=" + AuthorType +
                ", memberBadgeIconURL='" + MemberBadgeIconUrl + '\'' +
                ", bodyBackgroundColor=" + BodyBackgroundColor +
                ", bodyTextColor=" + BodyTextColor +
                ", headerBackgroundColor=" + HeaderBackgroundColor +
                ", headerTextColor=" + HeaderTextColor +
                ", authorNameTextColor=" + AuthorNameColor +
                ", purchaseAmount='" + PurchaseAmount + '\'' +
                ", stickerIconURL='" + StickerIconUrl + '\'' +
                ", backgroundColor=" + BackgroundColor +
                ", endBackgroundColor=" + EndBackgroundColor +
                ", durationSec=" + DurationSec +
                ", fullDurationSec=" + FullDurationSec +
                '}';
        }
    }
}
