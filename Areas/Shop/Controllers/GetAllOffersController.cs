using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Shop.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Shop.Controllers
{
    [Route("Shop/[action]")]
    [ApiController]
    public class GetAllOffersController : ControllerBase
    {
        private ApplicationDBContext _context;

        public GetAllOffersController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("GetAllOffers")]
        public IActionResult getAllShopOffers()
        {
            try
            {
                User user = TokenUserManager.getUserFromToken(HttpContext, _context);
                if (!user.open) return BadRequest(new { error = "YoureBanned" });
                List<OfferShop> offersToShow = new List<OfferShop>();

                _context.ShopOffers.ToList().ForEach(offer =>
                {
                    _context.Entry(offer).Reference("type").Load();
                    offersToShow.Add(new OfferShop
                    {
                        productId = offer.offerCode,
                        title = offer.title,
                        price = offer.price,
                        description = offer.description,
                        type = offer.type.name
                    });
                });

                return Ok(new { offers = offersToShow });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
