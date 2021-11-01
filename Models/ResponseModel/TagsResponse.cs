using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models.ResponseModel
{
    public class TagsResponse
    {
        public List<Tag> Tags { get; set; }
        public List<TagGroup> TagGroups { get; set; }
    }
}
