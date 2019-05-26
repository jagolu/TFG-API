using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class ManagePassword
    {
        [Required]
        public string name { get; set; }

        public string newPassword { get; set; }
        public string oldPassword { get; set; }
    }
}
