﻿using API.Data.Models;
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
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<Limitations> Limitations { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<MatchDay> MatchDays { get; set; }
        public DbSet<OfferType> OfferTypes { get; set; }
        public DbSet<ShopOffer> ShopOffers { get; set; }
        public DbSet<FootballBet> FootballBets { get; set; }
        public DbSet<TypeFootballBet> TypeFootballBet { get; set; }
        public DbSet<TypePay> TypePay { get; set; }
        public DbSet<UserBet> UserBet { get; set; }


        protected override void OnModelCreating(ModelBuilder mb)
        {
            onCreateUser(mb);
            onCreateUserToken(mb);
            onCreateGroup(mb);
            onCreateUserGroup(mb);
            onCreateMathDay(mb);
            onCreateFootballBet(mb);
            onCreateUserBet(mb);
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
                .HasIndex(u => u.publicId)
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
                .HasKey(ug => new { ug.userId, ug.groupId });

            mb.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.groups)
                .HasForeignKey(ug => ug.userId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.users)
                .HasForeignKey(ug => ug.groupId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void onCreateMathDay(ModelBuilder mb)
        {
            mb.Entity<MatchDay>()
                .HasAlternateKey(md => new { md.CompetitionId, md.number, md.HomeTeamId, md.AwayTeamId });

            mb.Entity<MatchDay>()
                .HasOne(md => md.Competition)
                .WithMany(c => c.matchDays)
                .HasForeignKey(md => md.CompetitionId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<MatchDay>()
                .HasOne(md => md.HomeTeam)
                .WithMany(t => t.homeMatchDays)
                .HasForeignKey(md => md.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<MatchDay>()
                .HasOne(md => md.AwayTeam)
                .WithMany(t => t.awayMatchDays)
                .HasForeignKey(md => md.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void onCreateFootballBet(ModelBuilder mb)
        {
            mb.Entity<FootballBet>()
                .HasAlternateKey(fb => new { fb.matchdayId, fb.groupId });

            mb.Entity<FootballBet>()
                .HasOne(fb => fb.MatchDay)
                .WithMany(md => md.bets)
                .HasForeignKey(fb => fb.matchdayId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<FootballBet>()
                .HasOne(fb => fb.Group)
                .WithMany(g => g.bets)
                .HasForeignKey(fb => fb.groupId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void onCreateUserBet(ModelBuilder mb)
        {
            mb.Entity<UserBet>()
                .HasAlternateKey(fb => new { fb.FootballBetId, fb.userId });

            mb.Entity<UserBet>()
                .HasOne(ub => ub.FootballBet)
                .WithMany(fb => fb.userBets)
                .HasForeignKey(ub => ub.FootballBetId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<UserBet>()
                .HasOne(ub => ub.User)
                .WithMany(fb => fb.bets)
                .HasForeignKey(ub => ub.userId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
