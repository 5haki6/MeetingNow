using MeetingNow.Models;
using MeetingNow.Models.ReqestModels;
using MeetingNow.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
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
            foreach(int i in eventModel.Tags)
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
    }
}
