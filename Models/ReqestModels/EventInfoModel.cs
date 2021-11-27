using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models.ReqestModels
{
    public class EventInfoModel
    {
       public int EventId { get; set; }
       public List<int> TagsId { get; set; }
    }
}
