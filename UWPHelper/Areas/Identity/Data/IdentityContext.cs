using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UWPHelper.Areas.Identity.Data;

namespace UWPHelper.Models
{
    public class IdentityContext : IdentityDbContext<UWPHelperUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        public DbSet<History> HistorySet { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            var history = builder.Entity<History>();
            history.ToTable("History").HasKey(r => r.ID);
            history.Property(r => r.UserID).IsRequired();

            history.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserID).IsRequired();
        }
    }
}
