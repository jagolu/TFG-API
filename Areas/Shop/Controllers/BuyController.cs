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
                return BadRequest();
            }

            //Realizar la comprobacion del pago
            if (item.productId == "GROUPCAPACITY_PLUS_5") result = addGroupCapacity(item.group, 5);
            if(item.productId == "GROUPCAPACITY_PLUS_25") result = addGroupCapacity(item.group, 25);
            if (item.productId == "GROUPCAPACITY_PLUS_50") result = addGroupCapacity(item.group, 50);
            if (item.productId == "GROUP_ADD_PASSWORD") {
                result = enablePasswordToGroup(item.group);
                if(result) return Ok(new { success = "EnabledGroupPassword" });
            };

            if (result) return Ok(new { success = "SuccesfullBuy" });

            //Return the money?¿
            return BadRequest(new { error = "ErrorBuy" });
        }


        private bool addGroupCapacity(string groupName, int capacityToAdd)
        {

            Data.Models.Group group = checkGroup(groupName);

            if (group == null) return false;

            group.capacity = group.capacity + capacityToAdd;

            try
            {
                _context.Group.Update(group);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool enablePasswordToGroup(string groupName)
        {
            Data.Models.Group group = checkGroup(groupName);

            if (group == null) return false;

            try
            {
               // if (group.canPutPassword) return false; //The group already can put a password on it

//                group.canPutPassword = true;
                _context.Group.Update(group);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Data.Models.Group checkGroup(string groupName)
        {
            if (groupName == null)
            {
                return null;
            }

            var group = _context.Group.Where(g => g.name == groupName);

            if (group.Count() != 1)
            {
                return null;
            }
            try
            {
                return _context.Group.Where(g => g.name == groupName).First();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
