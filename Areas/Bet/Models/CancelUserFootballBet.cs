using System.ComponentModel.DataAnnotations;

namespace API.Areas.Bet.Models
{
    public class CancelUserFootballBet
    {
        [Required]
        public string groupName { get; set; }

        [Required]
        public string footballBet { get; set; }

        [Required]
        public string userBet { get; set; }
    }
}
