using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MEMO_JOB.Models.JobSeekers.AccountViewModels
{
    public class SendingEmailConfirmViewModel
    {
        [Required]
        public string Email { get; set; }

    }
}
