using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MEMO_JOB.Models.Recruiters.ManageViewModels;
using MEMO_JOB.Models.Recruiters;
using MEMO_JOB.Models.Recruiters.AccountViewModels;

namespace MEMO_JOB.Models
{
    public partial class MEMO_JOBContext : DbContext
    {
        public virtual DbSet<AppJobRecruitersCompany> AppJobRecruitersCompany { get; set; }
        public virtual DbSet<AppJobRoleClaims> AppJobRoleClaims { get; set; }
        public virtual DbSet<AppJobRoles> AppJobRoles { get; set; }
        public virtual DbSet<AppJobUserClaims> AppJobUserClaims { get; set; }
        public virtual DbSet<AppJobUserExternalLogin> AppJobUserExternalLogin { get; set; }
        public virtual DbSet<AppJobUserRoles> AppJobUserRoles { get; set; }
        public virtual DbSet<AppJobUsers> AppJobUsers { get; set; }
        public virtual DbSet<AppJobUserTokens> AppJobUserTokens { get; set; }

        public MEMO_JOBContext(DbContextOptions<MEMO_JOBContext> options)
                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppJobRecruitersCompany>(entity =>
            {
                entity.HasKey(e => e.CompanyIdd);

                entity.ToTable("APP_JOB_RECRUITERS_COMPANY");

                entity.Property(e => e.CompanyDateCreated).HasColumnType("datetime");

                entity.Property(e => e.CompanyDateUpdated).HasColumnType("datetime");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CompanyPublishFrom).HasColumnType("datetime");

                entity.Property(e => e.CompanyPublishTo).HasColumnType("datetime");

                entity.Property(e => e.CompanySize).HasMaxLength(200);

                entity.Property(e => e.CompanyTaxcode).HasMaxLength(100);

                entity.Property(e => e.CompanyType).HasMaxLength(500);

                entity.Property(e => e.CompanyWebsite).HasMaxLength(200);
            });

            modelBuilder.Entity<AppJobRoleClaims>(entity =>
            {
                entity.ToTable("APP_JOB_ROLE_CLAIMS");

                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppJobRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AppJobRoles>(entity =>
            {
                entity.ToTable("APP_JOB_ROLES");

                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AppJobUserClaims>(entity =>
            {
                entity.ToTable("APP_JOB_USER_CLAIMS");

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppJobUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AppJobUserExternalLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.ToTable("APP_JOB_USER_EXTERNAL_LOGIN");

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppJobUserExternalLogin)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AppJobUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("APP_JOB_USER_ROLES");

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppJobUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppJobUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AppJobUsers>(entity =>
            {
                entity.ToTable("APP_JOB_USERS");

                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AppJobUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.ToTable("APP_JOB_USER_TOKENS");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppJobUserTokens)
                    .HasForeignKey(d => d.UserId);
            });
        }

        //public DbSet<MEMO_JOB.Models.Recruiters.ManageViewModels.RecruiterAccountManagementViewModel> RecruiterAccountManagementViewModel { get; set; }

        public DbSet<MEMO_JOB.Models.Recruiters.SubRecruiterUser> SubRecruiterUser { get; set; }

        //public DbSet<MEMO_JOB.Models.Recruiters.AccountViewModels.RegisterViewModel> RegisterViewModel { get; set; }
    }
}
