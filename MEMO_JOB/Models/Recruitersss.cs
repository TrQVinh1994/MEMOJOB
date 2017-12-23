using MEMO_JOB.Models.JobSeekers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MEMO_JOB.Models
{
    public class Recruitersss
    {
        public int ids{ get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
