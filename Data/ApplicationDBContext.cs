using API.Models;
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

        protected override void OnModelCreating(ModelBuilder mb)
        {
            onCreateUser(mb);
            onCreateUserToken(mb);
            onCreateUserGroup(mb);
        }

        private void onCreateUser(ModelBuilder mb)
        {
            mb.Entity<User>()
                .HasIndex(u => u.tokenValidation)
                .IsUnique();

            mb.Entity<User>()
                .HasIndex(u => u.email)
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
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<UserToken>()
                .HasIndex(ut => ut.refreshToken)
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
    }
}
