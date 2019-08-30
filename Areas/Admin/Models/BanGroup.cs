using System.ComponentModel.DataAnnotations;

namespace API.Areas.Admin.Models
{
    /// <summary>
    /// The order to ban or unban a group
    /// </summary>
    public class BanGroup
    {
        [Required]
        /// <value>The name of the group to ban</value>
        public string groupName { get; set; }

        [Required]
        /// <value>True to ban the group, false otherwise</value>
        public bool ban { get; set; }
    }
}
