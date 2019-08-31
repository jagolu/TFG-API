using System.ComponentModel.DataAnnotations;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// Necesary info to cancel a user fb 
    /// </summary>
    public class CancelUserFootballBet
    {
        [Required]
        /// <value>The name of the group where the fb belongs to</value>
        public string groupName { get; set; }

        [Required]
        /// <value>The id of the fb</value>
        public string footballBet { get; set; }

        [Required]
        /// <value>The id of the user fb</value>
        public string userBet { get; set; }
    }
}
