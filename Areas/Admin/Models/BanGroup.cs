using System.ComponentModel.DataAnnotations;

namespace API.Areas.Admin.Models
{
    public class BanGroup
    {
        [Required]
        public string groupName { get; set; }

        [Required]
        public bool ban { get; set; }
    }
}
