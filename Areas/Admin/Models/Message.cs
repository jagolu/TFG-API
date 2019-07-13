using System.ComponentModel.DataAnnotations;

namespace API.Areas.Admin.Models
{
    public class Message
    {
        [Required]
        [MaxLength(256)]
        [MinLength(5)]
        public string message { get; set; }
    }
}
