using System.ComponentModel.DataAnnotations;

namespace API.Areas.DirectMessages.Models
{
    public class SendDMMessage
    {
        [Required]
        public string dmId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string message { get; set; }
    }
}
