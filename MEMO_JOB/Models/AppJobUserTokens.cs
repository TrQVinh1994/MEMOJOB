using System;
using System.Collections.Generic;

namespace MEMO_JOB.Models
{
    public partial class AppJobUserTokens
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public AppJobUsers User { get; set; }
    }
}
