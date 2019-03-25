using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<UserPermission> UserPermission { get; set; }
        public DbSet<UserToken> UserToken { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            onCreateUser(mb);
            onCreateUserPermission(mb);
            onCreateUserToken(mb);
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

        private void onCreateUserPermission(ModelBuilder mb)
        {
            mb.Entity<UserPermission>()
                .HasKey(up => new { up.userId, up.permissionId });

            mb.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.permissions)
                .HasForeignKey(up => up.userId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.users)
                .HasForeignKey(up => up.permissionId)
                .OnDelete(DeleteBehavior.Restrict);
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
    }
}
