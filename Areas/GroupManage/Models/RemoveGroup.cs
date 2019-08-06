using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class RemoveGroup
    {
        [Required] 
        public string name { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string userPassword { get; set; }
    }
}
