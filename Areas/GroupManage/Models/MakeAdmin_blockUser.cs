using System.ComponentModel.DataAnnotations;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// Info to make/unmake admin or block/unblock a user
    /// </summary>
    public class MakeAdmin_blockUser
    {
        [Required]
        /// <value>The public id of the user to make/unmake/block/unblock</value>
        public string publicId { get; set; }

        [Required]
        /// <value>The name of the group</value>
        public string groupName { get; set; }

        [Required]
        /// <value>True to make admin/block the user, false to unmake admin/unblock the user</value>
        public bool make_unmake { get; set; }
    }
}
