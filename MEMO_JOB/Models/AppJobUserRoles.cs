using System;
using System.Collections.Generic;

namespace MEMO_JOB.Models
{
    public partial class AppJobUserRoles
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public AppJobRoles Role { get; set; }
        public AppJobUsers User { get; set; }
    }
}
