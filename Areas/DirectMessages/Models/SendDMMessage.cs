using System.ComponentModel.DataAnnotations;

namespace API.Areas.DirectMessages.Models
{
    public class SendDMMessage
    {
        [Required]
        public string dmId { get; set; }

        [Required]
        public string message { get; set; }
    }
}
