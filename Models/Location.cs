using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class Location
    {
        public Location(float x, float y, string address)
        {
            X = x;
            Y = y;
            Address = address;
        }

        public int LocationId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Address { get; set; }
    }
}
