using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Log;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<WorkOrder>()
                .HasOne(w => w.AppUser)
                .WithMany(a => a.WorkOrder)
                .HasForeignKey(w => w.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApiLog>()
                .HasOne(al => al.AppUser)
                .WithMany(a => a.ApiLog)
                .HasForeignKey(al => al.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ErrorLog>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            // 設定預設角色和角色權限
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "Admin", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "AuthorizedUser", Name = "AuthorizedUser", NormalizedName = "AUTHORIZEDUSER" },
                new IdentityRole { Id = "GeneralUser", Name = "GeneralUser", NormalizedName = "GENERALUSER" }
            );

            builder.Entity<IdentityRoleClaim<string>>().HasData(
                new IdentityRoleClaim<string> { Id = -1, RoleId = "Admin", ClaimType = "Permission", ClaimValue = "Edit" },
                new IdentityRoleClaim<string> { Id = -2, RoleId = "Admin", ClaimType = "Permission", ClaimValue = "Delete" },
                new IdentityRoleClaim<string> { Id = -3, RoleId = "AuthorizedUser", ClaimType = "Permission", ClaimValue = "Edit" }
            );

        }

        public static async Task SeedAdminUserAsync(UserManager<AppUser> userManager)
        {
            var adminUserName = "admin";
            var adminPassword = "admin";
            var adminUser = await userManager.FindByNameAsync(adminUserName);
            if (adminUser == null)
            {
                adminUser = new AppUser { 
                    UserName = adminUserName, 
                    Email = "pengrenwei89@gmail.com", 
                    EmailConfirmed = true 
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }


        }


    }
}