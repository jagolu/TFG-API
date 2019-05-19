using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class MakeAdmin
    {
        [Required]
        public string publicId { get; set; }

        [Required]
        public string groupName { get; set; }

        [Required]
        public bool makeAdmin { get; set; }
    }
}
