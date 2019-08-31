using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A type pay for fb
    /// </summary>
    public class TypePay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The type of the pay type</value>
        public Guid id { get; set; }

        [Required]
        [MaxLength(64)]
        /// <value>The name of the pay type</value>
        public string name { get; set; }

        [Required]
        [MaxLength(256)]
        /// <value>The description of the pay type</value>
        public string description { get; set; }

        [Required]
        /// <value>The win rate for the pay type</value>
        public double winRate { get; set; }

        [Required]
        /// <value>The loss/cancel rate for the pay type</value>
        public double winLoseCancel { get; set; }
    }
}
