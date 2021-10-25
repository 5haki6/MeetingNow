using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string ImagePath { get; set; }
        public string UserName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [NotMapped]
        public int? Age 
        { 
            get 
            {
                if(DateOfBirth.HasValue)
                {
                    return DateTime.Now.Year - DateOfBirth.Value.Year;
                }
                return null;
            } 
        }
        public string SelfInfo { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Event> Events { get; set; }
    }
}
