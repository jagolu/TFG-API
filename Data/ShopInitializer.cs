using API.Areas.Shop.Models;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public static class ShopInitializer
    {
        public static void Initialize(ApplicationDBContext context)
        {
            try
            {
                List<OfferType> types = initializeOfferTypes(context);
                initializeShop(context, types);
            }
            catch (Exception) { }
        }

        private static void initializeShop(ApplicationDBContext context, List<OfferType> types)
        {
            var offers = new ShopOffer[]
            {
                new ShopOffer
                {
                    offerCode = "GROUPCAPACITY_PLUS_5",
                    title = "GROUP CAPACITY + 5 MEMBERS",
                    price = 1,
                    description = "Add 5 members of capacity to your actual group capacity!",
                    type = types.Where(t => t.name == "Group").First()
                },
                new ShopOffer
                {
                    offerCode = "GROUPCAPACITY_PLUS_25",
                    title = "GROUP CAPACITY + 25 MEMBERS",
                    price = 5,
                    description = "Add 25 members of capacity to your actual group capacity!",
                    type = types.Where(t => t.name == "Group").First()
                },
                new ShopOffer
                {
                    offerCode = "GROUPCAPACITY_PLUS_50",
                    title = "GROUP CAPACITY + 50 MEMBERS",
                    price = 10,
                    description = "Add 50 members of capacity to your actual group capacity!",
                    type = types.Where(t => t.name == "Group").First()
                },
                new ShopOffer
                {
                    offerCode = "GROUP_ADD_PASSWORD",
                    title = "MAKE YOUR GROUP PRIVATE",
                    price = 3,
                    description = "Make your group private adding a password to join it!",
                    type = types.Where(t => t.name == "Group").First()
                },
                new ShopOffer
                {
                    offerCode = "USER_PRUEBA",
                    title = "Prueba User offer",
                    price = 10,
                    description = "This is a test",
                    type = types.Where(t => t.name == "User").First()
                }
            };

            foreach(ShopOffer so in offers)
            {
                if(context.ShopOffers.Where(cso => cso.title == so.title).Count() == 0)
                {
                    context.ShopOffers.Add(so);
                }
            }

            context.SaveChanges();
        }

        private static List<OfferType> initializeOfferTypes(ApplicationDBContext context)
        {
            var types = new OfferType[]
            {
                new OfferType{name = "Group"},
                new OfferType{name = "User"}
            };

            foreach(OfferType o in types)
            {
                if(context.OfferTypes.Where(ot => ot.name == o.name).Count() == 0)
                {
                    context.OfferTypes.Add(o);
                }
            }

            context.SaveChanges();

            return context.OfferTypes.ToList();
        }
    }
}
