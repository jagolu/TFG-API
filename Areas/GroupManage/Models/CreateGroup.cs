using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    public class CreateGroup
    {
        [Required]
        [MaxLength(20)]
        [MinLength(4)]
        public string name { get; set; }

        [Required]
        public bool type { get; set; } //1 Oficial | 0 Virtual
    }
}
