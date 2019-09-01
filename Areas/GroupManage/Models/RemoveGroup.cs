using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// The info to remove a group
    /// </summary>
    public class RemoveGroup
    {
        [Required] 
        /// <value>The name of the group</value>
        public string name { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The user password of the maker</value>
        public string userPassword { get; set; }
    }
}
