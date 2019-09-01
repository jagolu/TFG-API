using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// Necessary info to join in a group
    /// </summary>
    public class JoinGroup
    {
        [Required]
        /// <value>The name of the group</value>
        public string groupName { get; set; }
        
        /// <value>The password of the group</value>
        public string password { get; set; }
    }
}
