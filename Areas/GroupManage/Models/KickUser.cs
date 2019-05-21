using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class KickUser
    {
        [Required]
        public string publicId { get; set; }

        [Required]
        public string groupName { get; set; }
    }
}
