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

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "Admin",   // 手動設定 ID
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "User",    // 手動設定 ID
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }


    }
}