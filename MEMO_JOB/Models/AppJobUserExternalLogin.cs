using System;
using System.Collections.Generic;

namespace MEMO_JOB.Models
{
    public partial class AppJobUserExternalLogin
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }

        public AppJobUsers User { get; set; }
    }
}
