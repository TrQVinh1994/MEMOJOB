using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MEMO_JOB.Models.Recruiters.ManageViewModels
{
    public class RecruiterAccountManagementViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public string DateCreated { get; set; }
        public string Position { get; set; }
        public bool Subscribe { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
    }
}
