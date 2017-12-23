using MEMO_JOB.Models;
using MEMO_JOB.Models.Recruiters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MEMO_JOB.Data
{
    public class RecruiterDbContext : IdentityDbContext<RecruiterUser>
    {
        public RecruiterDbContext(DbContextOptions<RecruiterDbContext> options)
            : base(options)
        {
        }

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<IdentityUser>().Ignore(c => c.NormalizedUserName)
            //                              .Ignore(c => c.NormalizedEmail)
            //                              .Ignore(c => c.UserName);

            builder.Entity<IdentityUser>().ToTable("APP_JOB_RECRUITERS_USERS");
            builder.Entity<IdentityUserRole<string>>().ToTable("APP_JOB_RECRUITERS_USER_ROLES");
            builder.Entity<IdentityUserClaim<string>>().ToTable("APP_JOB_RECRUITERS_USER_CLAIMS");
            builder.Entity<IdentityUserLogin<string>>().ToTable("APP_JOB_RECRUITERS_USER_EXTERNAL_LOGIN");
            builder.Entity<IdentityUserToken<string>>().ToTable("APP_JOB_RECRUITERS_USER_TOKENS");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("APP_JOB_RECRUITERS_ROLE_CLAIMS");
            builder.Entity<IdentityRole<string>>().ToTable("APP_JOB_RECRUITERS_ROLES");
         }
    }
}
