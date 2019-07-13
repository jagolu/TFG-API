using System.ComponentModel.DataAnnotations;

namespace API.Areas.Admin.Models
{
    public class BanUser
    {
        [Required]
        public string publicUserId { get; set; }

        [Required]
        public bool ban { get; set; }
    }
}
