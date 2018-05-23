using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

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
            var hack = new Dictionary<string, string>();

            if (Directory.Exists("/run/secrets"))
            {
                hack.Add("Exists", "true");
                var fp = new PhysicalFileProvider("/run/secrets");
                foreach (var file in fp.GetDirectoryContents("/"))
                {
                    hack.Add(file.Name, file.IsDirectory.ToString());
                }
            }
            else
            {
                hack.Add("Exists", "false");                
            }

            return Ok(hack);
            //return new Random().Next(0, 1000) > 995 ? StatusCode(500, "Oh my gosh, it's full of stars") : Ok(_settings.Value.M2kSecret);
        }
    }
}