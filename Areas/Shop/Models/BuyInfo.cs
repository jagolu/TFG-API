using System.ComponentModel.DataAnnotations;

namespace API.Areas.Shop.Models
{
    public class BuyInfo
    {
        [Required]
        public string productId { get; set; }
        public string group { get; set; }
    }
}
