using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    /// <summary>
    /// The info of the refreshed session token
    /// </summary>
    public class RefreshRequest
    {
        [Required]
        /// <value>The token to refresh</value>
        public string token { get; set; }

        /// <value>The provider of the caller</value>
        public Boolean provider { get; set; } = false;
    }
}
