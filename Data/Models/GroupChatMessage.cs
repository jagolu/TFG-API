using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class GroupChatMessage
    {
        public Guid groupid { get; set; }
        public Group Group { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        [MaxLength(256)]
        public string publicUserid { get; set; }

        [Required]
        public Role role { get; set; }

        [Required]
        [StringLength(128)]
        public string message { get; set; }

        [Required]
        public DateTime time { get; set; }
    }
}
