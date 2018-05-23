using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace TodoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        private readonly IOptions<StatusSettings> _settings;

        public StatusController(IOptions<StatusSettings> settings)
        {
            _settings = settings;
        }

        [HttpGet]
        public ObjectResult GetStatus()
        {
            return new Random().Next(0, 1000) > 995 ? StatusCode(500, "Oh my gosh, it's full of stars") : Ok(_settings.Value.M2kSecret);
        }
    }
}