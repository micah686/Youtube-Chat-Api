using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeChatApi.Models
{
    class Emoji
    {
        public string EmojiId { get;  set; }
        public List<string> Shortcuts { get;  set; } = new List<string>();
        public string SearchTerms { get;  set; }
        public string IconUrl { get;  set; }
        public bool IsCustomEmoji { get;  set; }

        public override string ToString()
        {
            return "Emoji{" +
                "emojiId='" + EmojiId + '\'' +
                ", shortcuts=" + Shortcuts +
                ", searchTerms=" + SearchTerms +
                ", iconURL='" + IconUrl + '\'' +
                ", isCustomEmoji=" + IsCustomEmoji +
                '}';
        }
    }
}
