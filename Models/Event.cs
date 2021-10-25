using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public User Owner { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string ImagePath { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public List<Profile> Users { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
