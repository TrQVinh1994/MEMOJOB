using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MEMO_JOB.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Idd { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime DateUpdated { set; get; }
        public DateTime LastLoginDate { set; get; }
        public DateTime PasswordChangeDate { set; get; }

        [Required]
        public string FullName { set; get; }
        public bool Subscribe { set; get; }
        public string Address { set; get; }
        public string City { set; get; }
        public string Nation { set; get; }
        public string Avatar { set; get; }
        public DateTime Birthday { set; get; }
        public string Gender { set; get; }
        public bool MaritalStatus { set; get; }

        //Company
        [ForeignKey("AppJobRecruitersCompany")]
        public long CompanyIdd { get; set; }
        public string Fax { get; set; }
        public string Position { get; set; }
        public DateTime PublishFrom { get; set; }
        public DateTime PublishTo { get; set; }
        public string PhoneDesk { get; set; }
        public AppJobRecruitersCompany AppJobRecruitersCompany { get; set; }

    }
}
