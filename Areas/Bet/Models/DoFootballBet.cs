using System.ComponentModel.DataAnnotations;

namespace API.Areas.Bet.Models
{
    public class DoFootballBet
    {
        [Required]
        public string groupName { get; set; }

        [Required]
        public string footballbet { get; set; }

        [Required]
        public int bet { get; set; }

        public int? homeGoals { get; set; }
        public int? awayGoals { get; set; }
        public int? winner { get; set; }
    }
}
