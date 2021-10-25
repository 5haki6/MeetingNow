using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public Event Event { get; set; }
        public int EventId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public int LikeCounter { get; set; }
        public int DislikeCounter { get; set; }
    }
}
