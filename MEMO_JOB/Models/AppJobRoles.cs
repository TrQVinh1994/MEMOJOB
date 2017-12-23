using System;
using System.Collections.Generic;

namespace MEMO_JOB.Models
{
    public partial class AppJobRoles
    {
        public AppJobRoles()
        {
            AppJobRoleClaims = new HashSet<AppJobRoleClaims>();
            AppJobUserRoles = new HashSet<AppJobUserRoles>();
        }

        public string Id { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Discriminator { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public ICollection<AppJobRoleClaims> AppJobRoleClaims { get; set; }
        public ICollection<AppJobUserRoles> AppJobUserRoles { get; set; }
    }
}
