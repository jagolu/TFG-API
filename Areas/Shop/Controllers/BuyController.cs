using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Areas.Shop.Models;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Shop.Controllers
{
    [Route("Shop/[action]")]
    [ApiController]
    public class BuyController : ControllerBase
    {

        private ApplicationDBContext _context;

        public BuyController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("Buy")]
        public IActionResult buy([FromBody] BuyInfo item)
        {
            bool result = false;
            //TODO falta comprobacion de pago
            if(_context.ShopOffers.Where(so=> so.offerCode == item.productId).Count() != 1)
            {
                return BadRequest(new { error = "" });
            }

            //Realizar la comprobacion del pago
            if (item.productId == "GROUPCAPACITY_PLUS_5") result = addGroupCapacity(item.group, 5);
            if(item.productId == "GROUPCAPACITY_PLUS_25") result = addGroupCapacity(item.group, 25);
            if (item.productId == "GROUPCAPACITY_PLUS_50") result = addGroupCapacity(item.group, 50);

            if (result) return Ok(new { success = "SuccesfullBuy" });

            //Return the money?¿
            return BadRequest(new { error = "ErrorBuy" });
        }


        private bool addGroupCapacity(string groupName, int capacityToAdd)
        {
            if(groupName == null)
            {
                return false;
            }

            var group = _context.Group.Where(g => g.name == groupName);

            if(group.Count() != 1)
            {
                return false;
            }

            group.First().capacity = group.First().capacity + capacityToAdd;

            try
            {
                _context.Group.Update(group.First());
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
