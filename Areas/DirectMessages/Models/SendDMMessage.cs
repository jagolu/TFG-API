using System.ComponentModel.DataAnnotations;

namespace API.Areas.DirectMessages.Models
{
    /// <summary>
    /// The info to send a message to a dm conversation
    /// </summary>
    public class SendDMMessage
    {
        [Required]
        /// <value>The id of the dm conversation</value>
        public string dmId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(510)]
        /// <value>The text of the message</value>
        public string message { get; set; }
    }
}
