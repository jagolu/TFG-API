using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class TypePay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        [MaxLength]
        public string description { get; set; }

        [Required]
        public double winRate { get; set; }
    }
}
