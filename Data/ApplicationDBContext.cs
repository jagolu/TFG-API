using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options) : base(options) { }

        

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<UserPermission> UserPermission { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            onCreateUser(mb);
            onCreateUserPermission(mb);

        }


        private void onCreateUser(ModelBuilder mb)
        {
            mb.Entity<User>()
                .HasIndex(u => u.tokenValidation)
                .IsUnique();
        }

        private void onCreateUserPermission(ModelBuilder mb)
        {
            mb.Entity<UserPermission>()
                .HasKey(x => new { x.userId, x.permissionId });

            mb.Entity<UserPermission>()
                .HasOne(x => x.User)
                .WithMany(y => y.permissions)
                .HasForeignKey(x => x.userId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<UserPermission>()
                .HasOne(x => x.Permission)
                .WithMany(y => y.users)
                .HasForeignKey(x => x.permissionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
