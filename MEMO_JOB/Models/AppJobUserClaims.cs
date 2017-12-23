using System;
using System.Collections.Generic;

namespace MEMO_JOB.Models
{
    public partial class AppJobUserClaims
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string UserId { get; set; }

        public AppJobUsers User { get; set; }
    }
}
