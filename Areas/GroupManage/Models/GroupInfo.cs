using System;

namespace API.Areas.GroupManage.Models
{
    public class GroupInfo
    {
        public string name { get; set; }
        public bool type { get; set; }
        public bool password { get; set; }
        public int placesOcupped { get; set; }
        public int totalPlaces { get; set; }
        public DateTime dateCreate { get; set; }
    }
}
