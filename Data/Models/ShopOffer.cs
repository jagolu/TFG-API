using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class ShopOffer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        public string offerCode { get; set; }

        [Required]
        public string title { get; set; }

        [Required]
        public double price { get; set; }

        [Required]
        public string description { get; set; }

        [Required]
        public OfferType type { get; set; }
    }
}
