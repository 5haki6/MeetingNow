using MeetingNow.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {
        ApplicationContext ApplicationContext;
        public TestController(ApplicationContext applicationContext)
        {
            ApplicationContext = applicationContext;
        }
        [HttpGet("Test")]
        public string Test()
        {
            return "Hello";
        }
    }
}
