using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
        public int EventId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public int LikeCounter { get; set; }
    }
}
