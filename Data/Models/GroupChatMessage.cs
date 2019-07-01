using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class GroupChatMessage
    {
        public Guid groupId { get; set; }
        public Group Group { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string publicUserId { get; set; }

        [Required]
        public Role role { get; set; }

        [Required]
        [StringLength(128)]
        public string message { get; set; }

        [Required]
        public DateTime time { get; set; }
    }
}
