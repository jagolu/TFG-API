using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class ManagePassword
    {
        [Required]
        public string name { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string newPassword { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string oldPassword { get; set; }
    }
}
