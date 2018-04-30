using Microsoft.AspNetCore.Mvc;
using System;

namespace TodoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        [HttpGet]
        public ObjectResult GetStatus()
        {
            return new Random().Next(0, 1000) > 995 ? StatusCode(500, "Oh my gosh, it's full of stars") : Ok("I'm OK");
        }
    }
}