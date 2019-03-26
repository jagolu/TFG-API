using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.Common.Models;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Common.Controllers
{
    [Route("Authorize/[action]")]
    [ApiController]
    public class HomeAuthorizeController : ControllerBase
    {
        private ApplicationDBContext _context;

        public HomeAuthorizeController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Authorize]
        [ActionName("Home")]
        public IActionResult Get()
        {
            //TODO get noticias para usuarios registrados
            /**
             * Si el usuario es un administrador saldran solo noticias generales
             * Actualizaciónes en GitHub mismo ¿CON UN CRON?
             * Noticias normales y variadas
             */
            List<Message> msgs = new List<Message>();
            msgs.Add(new Message { title = "titulo1", body = "asdfasdf", owner = "sys", time = DateTime.Now });
            msgs.Add(new Message { title = "titulo1", body = "asdfasdf", owner = "sys", time = DateTime.Now.AddMinutes(5) });
            msgs.Add(new Message { title = "titulo1", body = "asdfasdf", owner = "sys", time = DateTime.Now.AddMinutes(10) });
            msgs.Add(new Message { title = "titulo3", body = "body 3", owner = "sys", time = DateTime.Now.AddHours(1) });

            return Ok(msgs);
        }
    }
}
