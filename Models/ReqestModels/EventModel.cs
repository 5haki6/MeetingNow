using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models.ReqestModels
{
    public class EventModel
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string ImagePath { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Address { get; set; }
        public int[] TagsId { get; set; }
    }
}
