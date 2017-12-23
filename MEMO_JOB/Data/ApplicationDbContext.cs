using MEMO_JOB.Models;
using MEMO_JOB.Models.JobSeekers;
using MEMO_JOB.Models.Recruiters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MEMO_JOB.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<JobSeekerUser> JobSeekerUser { get; set; }
        public DbSet<RecruiterUser> RecruiterUser { get; set; }
        public DbSet<SubRecruiterUser> SubRecruiterUser { get; set; }
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<IdentityUser>().Ignore(c => c.NormalizedUserName)
            //                              .Ignore(c => c.NormalizedEmail)
            //                              .Ignore(c => c.UserName);

            //builder.Entity<IdentityUser>().ToTable("APP_JOB_SEEKERS_USERS")
            //                              .Ignore(c => c.UserName);
            //builder.Entity<IdentityUserRole<string>>().ToTable("APP_JOB_SEEKERS_USER_ROLES");
            //builder.Entity<IdentityUserClaim<string>>().ToTable("APP_JOB_SEEKERS_USER_CLAIMS");
            //builder.Entity<IdentityUserLogin<string>>().ToTable("APP_JOB_SEEKERS_USER_EXTERNAL_LOGIN");
            //builder.Entity<IdentityUserToken<string>>().ToTable("APP_JOB_SEEKERS_USER_TOKENS");
            //builder.Entity<IdentityRoleClaim<string>>().ToTable("APP_JOB_SEEKERS_ROLE_CLAIMS");
            //builder.Entity<IdentityRole<string>>().ToTable("APP_JOB_SEEKERS_ROLES");

            //builder.Entity<JobSeekerUser>().ToTable("abc");
            //builder.Entity<RecruiterUser>().ToTable("xyz");
            builder.Entity<IdentityUser>().ToTable("APP_JOB_USERS");
                                        //.Ignore(c => c.UserName);
            builder.Entity<IdentityRole<string>>().ToTable("APP_JOB_ROLES");
            builder.Entity<IdentityUserRole<string>>().ToTable("APP_JOB_USER_ROLES");
            builder.Entity<IdentityUserClaim<string>>().ToTable("APP_JOB_USER_CLAIMS");           
            builder.Entity<IdentityUserToken<string>>().ToTable("APP_JOB_USER_TOKENS");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("APP_JOB_ROLE_CLAIMS");
            builder.Entity<IdentityUserLogin<string>>().ToTable("APP_JOB_USER_EXTERNAL_LOGIN");
        }
    }
}
