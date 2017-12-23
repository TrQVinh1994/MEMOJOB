using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEMO_JOB.Models
{
    public partial class AppJobRecruitersCompany
    {
        [Key]
        public long CompanyIdd { get; set; }
        public string CompanyBanner { get; set; }
        public string CompanyName { get; set; }
        public string CompanySize { get; set; }
        public string CompanyType { get; set; }
        public string CompanyInfo { get; set; }
        public DateTime CompanyDateCreated { get; set; }
        public DateTime CompanyDateUpdated { get; set; }
        public string CompanyImage { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyTaxcode { get; set; }
        public string CompanyWebsite { get; set; }
        public bool CompanyActiveStatus { get; set; }
        public int CompanyMaxUsers { get; set; }
        public DateTime? CompanyPublishFrom { get; set; }
        public DateTime? CompanyPublishTo { get; set; }
        public int? CompanyPoints { get; set; }
    }
}
