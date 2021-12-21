using MeetingNow.Helpers;
using MeetingNow.Models;
using MeetingNow.Models.ReqestModels;
using MeetingNow.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        [Authorize]
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
            foreach (int i in eventModel.TagsId)
            {
                e.Tags.Add(applicationContext.Tags.FirstOrDefault(t => t.TagId == i));
            }
            applicationContext.Events.Add(e);
            applicationContext.SaveChanges();
            return Ok();
        }
        [Authorize]
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
        [Authorize]
        [HttpGet("getUsersEvents")]
        public IActionResult GetUsersEvents()
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
                List<int> ultraLast = result.Distinct().ToList();
                return Json(ultraLast);
                //return Ok("Damn that's good linq query");
            }
            return BadRequest("Damn that's bad linq query. -15");
        }
        [Authorize]
        [HttpPost("getEventById")]
        public IActionResult GetEventById([FromBody] EventsId eventsId)
        {
            List<Event> megaUltraLast = new List<Event>();
            foreach (int id in eventsId.UltraLast)
            {
                megaUltraLast.Add(applicationContext.Events.FirstOrDefault(e => e.EventId == id));
            }
            return Json(megaUltraLast);
        }

        [Authorize]
        [HttpPost("participateInEvent/{eventId}")]
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
        [Authorize]
        [HttpPost("editEvent")]
        public IActionResult EditEvent([FromBody] EventModel eventModel)
        {
            Event existingEvent = applicationContext.Events.Include(e => e.Tags).FirstOrDefault(e => e.EventId == eventModel.EventId);
            if (existingEvent != null)
            {
                existingEvent.Name = eventModel.Name == null ? existingEvent.Name : eventModel.Name;
                existingEvent.Info = eventModel.Info == null ? existingEvent.Info : eventModel.Info;
                existingEvent.ImagePath = eventModel.ImagePath == null ? existingEvent.ImagePath : eventModel.ImagePath;
                existingEvent.Tags = new List<Tag>();
                applicationContext.Events.Update(existingEvent);
                applicationContext.SaveChanges();
                foreach (int i in eventModel.TagsId)
                {
                    existingEvent.Tags.Add(applicationContext.Tags.FirstOrDefault(t => t.TagId == i));
                }
                applicationContext.Events.Update(existingEvent);
                applicationContext.SaveChanges();
                return Ok();
            }
            return BadRequest("Invalid event`s id");
        }
        [Authorize]
        [HttpGet("getOwnersEvents")]
        public IActionResult GetOwnersEvents()
        {
            User user = (User)HttpContext.Items["User"];
            List<Event> events = applicationContext.Events.Where(e => e.UserId == user.UserId).ToList();
            return Json(events);
        }
        [Authorize]
        [HttpDelete("deleteEvent/{id}")]
        public IActionResult DeleteEvent(int id)
        {
            Event existingEvent = applicationContext.Events.FirstOrDefault(e => e.EventId == id);
            if(existingEvent != null)
            {
                applicationContext.Events.Remove(existingEvent);
                applicationContext.SaveChanges();
                return Ok();
            }
            return BadRequest("Invalid event`s id");
        }
        [Authorize]
        [HttpGet("getTagsByEventId/{id}")]
        public IActionResult GetTagsByEventId(int id)
        {
            Event existingEvent = applicationContext.Events.Include(e => e.Owner).Include(e => e.Location).Include(e => e.Tags).FirstOrDefault(e => e.EventId == id);
            if (existingEvent != null)
            {
                List<int> tagsId = existingEvent.Tags.Select(e => e.TagId).ToList();
                return Json(tagsId);
            }
            return BadRequest("Invalid event`s id");
        }
        [Authorize]
        [HttpPost("getEvent")]
        public IActionResult GetEvent([FromBody] EventInfoModel eventInfoModel)
        {
            Event existingEvent = applicationContext.Events.Include(e => e.Owner).Include(e => e.Location)
                .FirstOrDefault(e => e.EventId == eventInfoModel.EventId);
            if(existingEvent != null)
            {
                EventResponse eventResponse = new EventResponse
                {
                    EventId = eventInfoModel.EventId,
                    Name = existingEvent.Name,
                    Info = existingEvent.Info,
                    ImagePath = existingEvent.ImagePath,
                    Date = existingEvent.Date,
                    Location = existingEvent.Location,
                    OwnerId = existingEvent.Owner.UserId,
                    OwnerLogin = existingEvent.Owner.Login,
                    Tags = new List<Tag>(),
                    Comments = applicationContext.Comments.Where(e => e.EventId == eventInfoModel.EventId).ToList(),
                    UsersId = new List<int>()
                };
                foreach(int i in eventInfoModel.TagsId)
                {
                    eventResponse.Tags.Add(applicationContext.Tags.Include(t => t.TagGroup).FirstOrDefault(t => t.TagId == i));
                }
                foreach(User u in applicationContext.Users)
                {
                    eventResponse.UsersId.Add(u.UserId);
                }
                return Json(eventResponse);
            }
            return BadRequest("Error =(");
        }
        [Authorize]
        [HttpPost("addComment")]
        public IActionResult AddComment([FromBody] CommentModel commentModel)
        {
            User user = (User)HttpContext.Items["User"];
            if(!String.IsNullOrEmpty(commentModel.Text))
            {
                Comment comment = new Comment()
                {
                    Text = commentModel.Text,
                    EventId = commentModel.EventId,
                    UserId = user.UserId
                };
                applicationContext.Comments.Add(comment);
                applicationContext.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        [Authorize]
        [HttpPost("filterByTags")]
        public IActionResult FilterByTags([FromBody] FilterTagsModel filterTagsModel)
        {
            if(filterTagsModel.TagsId != null)
            {
                List<Event> events = new List<Event>();

                foreach(int i in filterTagsModel.TagsId)
                {
                    Tag tag = applicationContext.Tags.FirstOrDefault(t => t.TagId == i);
                    Event existingEvent = applicationContext.Events.FirstOrDefault(e => e.Tags.Contains(tag));
                    if(existingEvent != null)
                    {
                        events.Add(existingEvent);
                    }
                }
                return Json(events);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("searchByTitle/{name}")]
        public IActionResult SearchByTitle(string name)
        {
            if(name == null)
            {
                name = "";
            }
            if(name != null)
            {
                List<Event> events = applicationContext.Events.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
                return Json(events);
            }
            return BadRequest();
        }
        [HttpGet("searchByTitle")]
        public IActionResult SearchByTitle()
        {
                List<Event> events = applicationContext.Events.ToList();
                return Json(events);
        }
        [Authorize]
        [HttpGet("setLike/{id}")]
        public IActionResult SetLike(int id)
        {
            Comment existingComment = applicationContext.Comments.FirstOrDefault(c => c.CommentId == id);
            if (existingComment != null)
            {
                User user = (User)HttpContext.Items["User"];
                UserRate existingUserRate = applicationContext.userRates.FirstOrDefault(ur => ur.CommentId == id && ur.UserId == user.UserId);
                if (existingUserRate == null)
                {
                    existingComment.LikeCounter++;
                    UserRate userRate = new UserRate
                    {
                        UserId = user.UserId,
                        CommentId = existingComment.CommentId,
                    };
                    applicationContext.userRates.Add(userRate);
                    applicationContext.SaveChanges();
                    return Ok();
                }
                else
                {
                    existingComment.LikeCounter--;
                    applicationContext.userRates.Remove(existingUserRate);
                    applicationContext.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("viewPastMeetings")]
        public IActionResult ViewPastMeetings()
        {
            User user = (User)HttpContext.Items["User"];
            Profile profile = applicationContext.Profiles.Include(p => p.Events).FirstOrDefault(p => p.UserId == user.UserId);
            List<Event> events = profile.Events.Where(e => e.Date < DateTime.Now).ToList();
            List<Event> finalEvents = new List<Event>();
            for(int i = 0; i < events.Count; i++)
            {
                Event newEvent = new Event
                {
                    EventId = events[i].EventId,
                    UserId = events[i].UserId,
                    Name = events[i].Name,
                    Date = events[i].Date,
                    ImagePath = events[i].ImagePath,
                    Info = events[i].Info,
                    Location = events[i].Location
                };
                finalEvents.Add(newEvent);
            }
            return Json(finalEvents);
        }
    }
}
