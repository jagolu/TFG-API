using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class MakeAdmin_blockUser
    {
        [Required]
        public string publicId { get; set; }

        [Required]
        public string groupName { get; set; }

        [Required]
        public bool make_unmake { get; set; }
    }
}
