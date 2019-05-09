using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class OfferType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        public string name { get; set; }
    }
}
