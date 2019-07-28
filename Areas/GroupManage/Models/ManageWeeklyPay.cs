using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class ManageWeeklyPay
    {
        [Required]
        public string groupName { get; set; }

        [Required]
        public int weeklyPay { get; set; }
    }
}
