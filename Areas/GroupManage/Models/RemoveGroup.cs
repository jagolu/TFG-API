using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class RemoveGroup
    {
        [Required] 
        public string name { get; set; }

        [Required]
        public string userPassword { get; set; }
    }
}
