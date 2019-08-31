using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.DirectMessages.Models
{
    /// <summary>
    /// Info to create a dm conversation
    /// </summary>
    public class CreateDMTitle
    {
        [Required]
        [MinLength(3)]
        [MaxLength(60)]
        /// <value>Title of the dm conversation</value>
        public string title { get; set; }

        [EmailAddress]
        /// <value>The receiver of the dm conversation</value>
        public String emailReceiver { get; set; }
    }
}
