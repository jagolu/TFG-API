using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A group
    /// </summary>
    public class Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the group</value>
        public Guid id { get; set; }

        [Required]
        [MaxLength(32)]
        /// <value>The name of the group</value>
        public string name { get; set; }

        [MaxLength(512)]
        /// <value>The password of the group</value>
        public string password { get; set; } = null;

        [Required]
        /// <value>False if the group is banned, false otherwise</value>
        public Boolean open { get; set; } = true;

        [Required]
        /// <value>The capacity of the group</value>
        public int capacity { get; set; } = 30;

        [Required]
        /// <value>The max of fb that the group can do in a week</value>
        public int maxWeekBets { get; set; } = 10;

        [Required]
        /// <value>The date when the group was created</value>
        public DateTime dateCreated { get; set; } = DateTime.Today;

        [Required]
        /// <value>The coins that the group pay in a week</value>
        public int weeklyPay { get; set; } = 500;

    
        /// <value>The members of the group</value>
        public ICollection<UserGroup> users { get; set; } = new HashSet<UserGroup>();
        
        /// <value>The interactions of the users with that group</value>
        public ICollection<GroupInteraction> userInteractions { get; set; } = new HashSet<GroupInteraction>();
        
        /// <value>The fb launched in that group</value>
        public ICollection<FootballBet> bets { get; set; } = new HashSet<FootballBet>();
        
        /// <value>The chat messages in that group</value>
        public ICollection<GroupChatMessage> chatMessages { get; set; } = new HashSet<GroupChatMessage>();
        
        /// <value>The news for this group</value>
        public ICollection<New> news { get; set; } = new HashSet<New>();
    }
}
