using API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    /// <summary>
    /// The database context
    /// </summary>
    public class ApplicationDBContext: DbContext
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //
        
        /// <value>The users in the database</value>
        public DbSet<User> User { get; set; }
        
        /// <value>The roles in the database</value>
        public DbSet<Role> Role { get; set; }
        
        /// <value>The groups in the database</value>
        public DbSet<Group> Group { get; set; }
        
        /// <value>The group members in the database</value>
        public DbSet<UserGroup> UserGroup { get; set; }
        
        /// <value>The group chat messages in the database</value>
        public DbSet<GroupChatMessage> GroupChatMessage { get; set; }
        
        /// <value>The session tokens in the database</value>
        public DbSet<UserToken> UserToken { get; set; }
        
        /// <value>The football teams in the database</value>
        public DbSet<Team> Teams { get; set; }
        
        /// <value>The competitions in the database</value>
        public DbSet<Competition> Competitions { get; set; }
        
        /// <value>The matchdays in the database</value>
        public DbSet<MatchDay> MatchDays { get; set; }
        
        /// <value>The fb in the database</value>
        public DbSet<FootballBet> FootballBets { get; set; }
        
        /// <value>The type fb in the database</value>
        public DbSet<TypeFootballBet> TypeFootballBet { get; set; }
        
        /// <value>The pay types in the database</value>
        public DbSet<TypePay> TypePay { get; set; }
        
        /// <value>The user fb in the database</value>
        public DbSet<UserFootballBet> UserFootballBet { get; set; }
        
        /// <value>The news in the database</value>
        public DbSet<New> News { get; set; }
        
        /// <value>The direct messages conversations in the database</value>
        public DbSet<DirectMessageTitle> DirectMessagesTitle { get; set; }
        
        /// <value>The messages in direct messages conversations in the database</value>
        public DbSet<DirectMessageMessages> DirectMessagesMessages { get; set; }
        
        /// <value>The notifications in the database</value>
        public DbSet<Notifications> Notifications { get; set; }
        
        /// <value>The group interactions in the database</value>
        public DbSet<GroupInteraction> GroupInteractions { get; set; }


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options) : base(options) { }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Create all the database models
        /// </summary>
        /// <param name="mb">The database model builder</param>
        protected override void OnModelCreating(ModelBuilder mb)
        {
            onCreateUser(mb);
            onCreateUserToken(mb);
            onCreateGroup(mb);
            onCreateUserGroup(mb);
            onCreateMathDay(mb);
            onCreateCompetition(mb);
            onCreateTeam(mb);
            onCreateFootballBet(mb);
            onCreateUserFootballBet(mb);
            onCreateChatGroup(mb);
            onCreateNew(mb);
            onCreateDirectMessagesTitle(mb);
            onCreateDirectMessagesMessages(mb);
            onCreateNotifications(mb);
            onCreateGroupInteraction(mb);
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Create the user model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateUser(ModelBuilder mb)
        {
            mb.Entity<User>()
                .HasIndex(u => u.tokenValidation)
                .IsUnique();

            mb.Entity<User>()
                .HasIndex(u => u.tokenPassword)
                .IsUnique();

            mb.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();

            mb.Entity<User>()
                .HasIndex(u => u.publicid)
                .IsUnique();
        }

        /// <summary>
        /// Create the user token model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateUserToken(ModelBuilder mb)
        {
            mb.Entity<UserToken>()
                .HasKey(ut => new { ut.userId, ut.loginProvider });

            mb.Entity<UserToken>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.tokens)
                .HasForeignKey(ut => ut.userId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<UserToken>()
                .HasIndex(ut => ut.refreshToken)
                .IsUnique();
        }

        /// <summary>
        /// Create the group model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateGroup(ModelBuilder mb)
        {
            mb.Entity<Group>()
                .HasIndex(g => g.name)
                .IsUnique();
        }

        /// <summary>
        /// Create the group member model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateUserGroup(ModelBuilder mb)
        {
            mb.Entity<UserGroup>()
                .HasKey(ug => new { ug.userid, ug.groupid });

            mb.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.groups)
                .HasForeignKey(ug => ug.userid)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.users)
                .HasForeignKey(ug => ug.groupid)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Create the group chat model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateChatGroup(ModelBuilder mb)
        {
            mb.Entity<GroupChatMessage>()
                .HasKey(c => new { c.groupid, c.publicUserid, c.time});

            mb.Entity<GroupChatMessage>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.chatMessages)
                .HasForeignKey(ug => ug.groupid)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Create the matchday model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateMathDay(ModelBuilder mb)
        {
            mb.Entity<MatchDay>()
                .HasAlternateKey(md => new { md.competitionid, md.number, md.homeTeamId, md.awayTeamid, md.season});

            mb.Entity<MatchDay>()
                .HasOne(md => md.Competition)
                .WithMany(c => c.matchDays)
                .HasForeignKey(md => md.competitionid)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<MatchDay>()
                .HasOne(md => md.HomeTeam)
                .WithMany(t => t.homeMatchDays)
                .HasForeignKey(md => md.homeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<MatchDay>()
                .HasOne(md => md.AwayTeam)
                .WithMany(t => t.awayMatchDays)
                .HasForeignKey(md => md.awayTeamid)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Create the competition model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateCompetition(ModelBuilder mb)
        {
            mb.Entity<Competition>()
                .HasIndex(c => c.name)
                .IsUnique();
        }

        /// <summary>
        /// Create the team model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateTeam(ModelBuilder mb)
        {
            mb.Entity<Team>()
                .HasIndex(t => t.name)
                .IsUnique();
        }

        /// <summary>
        /// Create the fb model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateFootballBet(ModelBuilder mb)
        {
            mb.Entity<FootballBet>()
                .HasOne(fb => fb.MatchDay)
                .WithMany(md => md.bets)
                .HasForeignKey(fb => fb.matchdayid)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<FootballBet>()
                .HasOne(fb => fb.Group)
                .WithMany(g => g.bets)
                .HasForeignKey(fb => fb.groupid)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Create the user fb model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateUserFootballBet(ModelBuilder mb)
        {
            mb.Entity<UserFootballBet>()
                .HasAlternateKey(fb => new { fb.footballBetid, fb.userid, fb.dateDone });

            mb.Entity<UserFootballBet>()
                .HasOne(ub => ub.FootballBet)
                .WithMany(fb => fb.userBets)
                .HasForeignKey(ub => ub.footballBetid)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<UserFootballBet>()
                .HasOne(ub => ub.User)
                .WithMany(fb => fb.footballBets)
                .HasForeignKey(ub => ub.userid)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Create the new model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateNew(ModelBuilder mb)
        {
            mb.Entity<New>()
                .HasOne(n => n.Group)
                .WithMany(g => g.news)
                .HasForeignKey(n => n.groupid)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<New>()
                .HasOne(n => n.User)
                .WithMany(u => u.news)
                .HasForeignKey(n => n.userid)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Create the dm model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateDirectMessagesTitle(ModelBuilder mb)
        {
            mb.Entity<DirectMessageTitle>()
                .HasOne(dm => dm.Sender)
                .WithMany(u => u.directMessages)
                .HasForeignKey(dm => dm.senderid)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Create the dm messages model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateDirectMessagesMessages(ModelBuilder mb)
        {
            mb.Entity<DirectMessageMessages>()
                .HasOne(dm => dm.DirectMessageTitle)
                .WithMany(u => u.messages)
                .HasForeignKey(dm => dm.directMessageTitleid)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Create the notifications model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateNotifications(ModelBuilder mb)
        {
            mb.Entity<Notifications>()
                .HasOne(n => n.User)
                .WithMany(u => u.notifications)
                .HasForeignKey(n => n.userid)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Create the group interactions model
        /// </summary>
        /// <param name="mb">The database model builder</param>
        private void onCreateGroupInteraction(ModelBuilder mb)
        {
            mb.Entity<GroupInteraction>()
                .HasKey(ug => new { ug.userid, ug.groupid });

            mb.Entity<GroupInteraction>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.groupInteractions)
                .HasForeignKey(ug => ug.userid)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<GroupInteraction>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.userInteractions)
                .HasForeignKey(ug => ug.groupid)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
