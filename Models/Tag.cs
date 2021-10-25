using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public int TagGroupId { get; set; }

        public TagGroup TagGroup { get; set; }
        public string Name { get; set; }
        public List<Profile> Profiles { get; set; }
        public List<Event> Events { get; set; }
    }
}
