using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    public class DeleteUser
    {
        public string email { get; set; }

        public string password { get; set; }
    }
}
