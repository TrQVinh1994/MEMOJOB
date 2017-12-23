using System;
using System.Collections.Generic;

namespace MEMO_JOB.Models
{
    public partial class AppJobUsers
    {
        public AppJobUsers()
        {
            AppJobUserClaims = new HashSet<AppJobUserClaims>();
            AppJobUserExternalLogin = new HashSet<AppJobUserExternalLogin>();
            AppJobUserRoles = new HashSet<AppJobUserRoles>();
            AppJobUserTokens = new HashSet<AppJobUserTokens>();
        }

        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public string City { get; set; }
        public long CompanyIdd { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Fax { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public long? Idd { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? MaritalStatus { get; set; }
        public string Nation { get; set; }
        public DateTime? PasswordChangeDate { get; set; }
        public string PhoneDesk { get; set; }
        public string Position { get; set; }
        public DateTime? PublishFrom { get; set; }
        public DateTime? PublishTo { get; set; }
        public bool Subscribe { get; set; }
        public string Id { get; set; }
        public int AccessFailedCount { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Discriminator { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string NormalizedEmail { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; }

        public ICollection<AppJobUserClaims> AppJobUserClaims { get; set; }
        public ICollection<AppJobUserExternalLogin> AppJobUserExternalLogin { get; set; }
        public ICollection<AppJobUserRoles> AppJobUserRoles { get; set; }
        public ICollection<AppJobUserTokens> AppJobUserTokens { get; set; }
    }
}
