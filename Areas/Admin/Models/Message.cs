using System.ComponentModel.DataAnnotations;

namespace API.Areas.Admin.Models
{
    /// <summary>
    /// The body of a new
    /// </summary>
    public class Message
    {
        [Required]
        [MinLength(5)]
        [MaxLength(256)]
        /// <value>The message to put on the body of the new</value>
        public string message { get; set; }
    }
}
