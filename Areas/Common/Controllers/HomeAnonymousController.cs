using System;
using System.Collections.Generic;
using API.Areas.Common.Models;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Common.Controllers
{
    [Route("Anonymous/[action]")]
    [ApiController]
    public class HomeAnonymousController : ControllerBase
    {
        private ApplicationDBContext _context;

        public HomeAnonymousController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        [AllowAnonymous]
        [ActionName("Home")]
        public IActionResult Get()
        {
            //TODO get noticias para usuarios anónimos
            /**
             * Actualizaciónes en GitHub mismo ¿CON UN CRON?
             * Noticias normales y variadas
             */
            List<Message> msgs = new List<Message>();
            msgs.Add(new Message { title = "titulo1", body = "asdfasdf", owner = "sys" , time=DateTime.Now });
            msgs.Add(new Message { title = "titulo3", body = "body 3", owner = "sys", time =DateTime.Now.AddHours(1) });

            return Ok(msgs);
        }
    }
}
