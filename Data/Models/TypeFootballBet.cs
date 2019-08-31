using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A fb type
    /// </summary>
    public class TypeFootballBet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the fb type</value>
        public Guid id { get; set; }

        [Required]
        [MaxLength(64)]
        /// <value>The name of the fb type</value>
        public string name { get; set; }

        [Required]
        [MaxLength(256)]
        /// <value>The description of the fb type</value>
        public string description { get; set; }

        [Required]
        /// <value>The winrate of the fb type</value>
        public double winRate { get; set; }

        [Required]
        /// <value>The loss/cancel rate for the fb type</value>
        public double winLoseCancel { get; set; }
    }
}
