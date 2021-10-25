using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Address { get; set; }
    }
}
