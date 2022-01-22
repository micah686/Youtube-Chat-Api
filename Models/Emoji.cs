using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeChatApi.Models
{
    class Emoji
    {
        public string EmojiId { get; protected set; }
        public List<string> Shortcuts { get; protected set; }
        public string SearchTerms { get; protected set; }
        public string IconUrl { get; protected set; }
        public bool IsCustomEmoji { get; protected set; }

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
