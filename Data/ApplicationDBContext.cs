using API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<GroupChatMessage> GroupChatMessage { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<MatchDay> MatchDays { get; set; }
        public DbSet<FootballBet> FootballBets { get; set; }
        public DbSet<TypeFootballBet> TypeFootballBet { get; set; }
        public DbSet<TypePay> TypePay { get; set; }
        public DbSet<UserFootballBet> UserFootballBet { get; set; }
        public DbSet<New> News { get; set; }
        public DbSet<DirectMessageTitle> DirectMessagesTitle { get; set; }
        public DbSet<DirectMessageMessages> DirectMessagesMessages { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<GroupInteraction> GroupInteractions { get; set; }


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

        private void onCreateGroup(ModelBuilder mb)
        {
            mb.Entity<Group>()
                .HasIndex(g => g.name)
                .IsUnique();
        }

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

        private void onCreateCompetition(ModelBuilder mb)
        {
            mb.Entity<Competition>()
                .HasIndex(c => c.name)
                .IsUnique();
        }

        private void onCreateTeam(ModelBuilder mb)
        {
            mb.Entity<Team>()
                .HasIndex(t => t.name)
                .IsUnique();
        }

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

        private void onCreateDirectMessagesTitle(ModelBuilder mb)
        {
            mb.Entity<DirectMessageTitle>()
                .HasOne(dm => dm.Sender)
                .WithMany(u => u.directMessages)
                .HasForeignKey(dm => dm.senderid)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void onCreateDirectMessagesMessages(ModelBuilder mb)
        {
            mb.Entity<DirectMessageMessages>()
                .HasOne(dm => dm.DirectMessageTitle)
                .WithMany(u => u.messages)
                .HasForeignKey(dm => dm.directMessageTitleid)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void onCreateNotifications(ModelBuilder mb)
        {
            mb.Entity<Notifications>()
                .HasOne(n => n.User)
                .WithMany(u => u.notifications)
                .HasForeignKey(n => n.userid)
                .OnDelete(DeleteBehavior.Cascade);
        }

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
