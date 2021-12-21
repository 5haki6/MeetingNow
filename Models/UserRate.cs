using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class UserRate
    {
        public int UserRateId { get; set; }
        public int UserId { get; set; }
        public int CommentId { get; set; }
    }
}
