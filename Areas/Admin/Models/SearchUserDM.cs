namespace API.Areas.Admin.Models
{
    public class SearchUserDM
    {
        public SearchUserDM(Data.Models.User u)
        {
            this.email = u.email;
            this.username = u.nickname;
        }
        public string email { get; set; }
        public string username { get; set; }
    }
}
