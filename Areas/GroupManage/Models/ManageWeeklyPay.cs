using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// Info to manage the weekly pay of a group
    /// </summary>
    public class ManageWeeklyPay
    {
        [Required]
        /// <value>The name of the group</value>
        public string groupName { get; set; }

        [Required]
        /// <value>The new weekly pay</value>
        public int weeklyPay { get; set; }
    }
}
