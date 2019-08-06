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
        [MaxLength(64)]
        public string name { get; set; }

        [Required]
        [MaxLength(256)]
        public string description { get; set; }

        [Required]
        public double winRate { get; set; }

        [Required]
        public double winLoseCancel { get; set; }
    }
}
