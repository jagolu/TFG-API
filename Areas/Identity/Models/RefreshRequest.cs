using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    public class RefreshRequest
    {
        [Required]
        public string token { get; set; }

        public Boolean provider { get; set; } = false;
    }
}
