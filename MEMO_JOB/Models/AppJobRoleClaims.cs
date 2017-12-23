using System;
using System.Collections.Generic;

namespace MEMO_JOB.Models
{
    public partial class AppJobRoleClaims
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string RoleId { get; set; }

        public AppJobRoles Role { get; set; }
    }
}
