using System.ComponentModel.DataAnnotations;

namespace API.Areas.Admin.Models
{
    /// <summary>
    /// The order to ban or unban a user
    /// </summary>
    public class BanUser
    {
        [Required]
        /// <value>The public id of the user to ban</value>
        public string publicUserId { get; set; }

        [Required]
        /// <value>True to ban the user, false otherwise</value>
        public bool ban { get; set; }
    }
}
