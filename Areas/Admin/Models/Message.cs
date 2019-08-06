using System.ComponentModel.DataAnnotations;

namespace API.Areas.Admin.Models
{
    public class Message
    {
        [Required]
        [MinLength(5)]
        [MaxLength(256)]
        public string message { get; set; }
    }
}
