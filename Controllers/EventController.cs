using MeetingNow.Helpers;
using MeetingNow.Models;
using MeetingNow.Models.ReqestModels;
using MeetingNow.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Controllers
{
    [Route("api/{controller}")]
    public class EventController : Controller
    {
        ApplicationContext applicationContext;
        public EventController(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }
        [HttpPost("addEvent")]
        public IActionResult addEvent([FromBody] EventModel eventModel)
        {
            Event e = new Event
            {
                UserId = eventModel.UserId,
                Date = DateTime.Now,
                ImagePath = eventModel.ImagePath,
                Info = eventModel.Info,
                Name = eventModel.Name,
                Location = new Location(eventModel.X, eventModel.Y, eventModel.Address),
                Tags = new List<Tag>()
            };
            foreach(int i in eventModel.TagsId)
            {
                e.Tags.Add(applicationContext.Tags.FirstOrDefault(t => t.TagId == i));
            }
            applicationContext.Events.Add(e);
            applicationContext.SaveChanges();
            return Ok();
        }
        [HttpGet("addEvent")]
        public IActionResult addEvent()
        {
            TagsResponse tagsResponse = new TagsResponse()
            {
                Tags = applicationContext.Tags.ToList(),
                TagGroups = applicationContext.TagGroups.ToList()
            };
            return Json(tagsResponse);
        }

        [HttpGet("getEvents")]
        public IActionResult getEvents()
        {
            return Json(applicationContext.Events
                .Include(e => e.Users)
                .Include(e => e.Owner).ToList());
        }

        [HttpGet("getUsersEvents")]
        public IActionResult getUsersEvents()
        {
            User user = (User)HttpContext.Items["User"];
            Profile profile = applicationContext.Profiles.Include(p => p.Events).Include(t => t.Tags).FirstOrDefault(p => p.UserId == user.UserId);
            if (profile != null)
            {
                List<int> tagGroupsId = applicationContext.Tags
                    .Include(tg => tg.TagGroup).Where(t => profile.Tags.Contains(t))
                    .Select(tg => tg.TagGroupId).Distinct().ToList();
                List<Event> events = applicationContext.Events
                    .Include(t => t.Tags).ToList();
                List<int> result = new List<int>();
                foreach (Event e in events)
                {
                    foreach (Tag t in e.Tags)
                    {
                        if (tagGroupsId.Contains(t.TagGroupId))
                        {
                            result.Add(e.EventId);
                        }

                    }

                }
                applicationContext.SaveChanges();
                List<int> ultraLast = result.Distinct().ToList();
                List<Event> megaUltraLast = new List<Event>();
                foreach (int id in ultraLast)
                {
                    megaUltraLast.Add(applicationContext.Events.FirstOrDefault(e => e.EventId == id));
                }
                return Json(megaUltraLast);
                //return Ok("Damn that's good linq query");
            }
            return BadRequest("Damn that's bad linq query. -15");
        }

        [Authorize]
        [HttpPost("participateInEvent")]
        public IActionResult participateInEvent(int eventId)
        {
            Event existingEvent = applicationContext.Events.Include(e => e.Users).FirstOrDefault(e => e.EventId == eventId);
            if (existingEvent != null)
            {
                User user = (User)HttpContext.Items["User"];
                Profile profile = applicationContext.Profiles.Include(p => p.Events).FirstOrDefault(p => p.UserId == user.UserId);
                existingEvent.Users.Add(profile);
                profile.Events.Add(existingEvent);
                applicationContext.SaveChanges();
                return Ok();
            }
            return BadRequest("No such event");
        }
    }
}
