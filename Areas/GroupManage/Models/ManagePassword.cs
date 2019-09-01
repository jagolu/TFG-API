using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// Info to change the password of the group
    /// </summary>
    public class ManagePassword
    {
        [Required]
        /// <value>The name of the group</value>
        public string name { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The new password of the group</value>
        public string newPassword { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The old password of the group</value>
        public string oldPassword { get; set; }
    }
}
