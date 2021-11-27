using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models.ResponseModel
{
    public class EventResponse
    {
        public int EventId { get; set; }
        public int OwnerId { get; set; }
        public string OwnerLogin { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Info { get; set; }
        public DateTime Date { get; set; }
        public Location Location { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
