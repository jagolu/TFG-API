using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class JoinGroup
    {
        [Required]
        public string groupName { get; set; }
        public string password { get; set; }
    }
}
