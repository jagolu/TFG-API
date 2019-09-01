using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// Necessary info to get a user out of a group
    /// </summary>
    public class KickUser
    {
        [Required]
        /// <value>The public id of the user who is gonna be kicked</value>
        public string publicId { get; set; }

        [Required]
        /// <value>The name of the group</value>
        public string groupName { get; set; }
    }
}
