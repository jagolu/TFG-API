using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    public class News
    {
        public Guid groupId { get; set; } //PrimaryKey
        public Guid userId { get; set; } //PrimaryKey
        public DateTime time { get; set; } = DateTime.Now; //PrimaryKey

        [Required]
        [MaxLength(30)]
        public string title { get; set; }

        [Required]
        [MaxLength]
        public string body { get; set; }
    }
}
