using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MEMO_JOB.Models.Recruiters.ManageViewModels
{
    public class CompanyIndexViewModel
    {
        public string CompanyBanner { get; set; }
        public string CompanyName { get; set; }
        public string CompanySize { get; set; }
        public string CompanyType { get; set; }
        public string CompanyInfo { get; set; }
        public string CompanyImage { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyTaxcode { get; set; }
        public string CompanyWebsite { get; set; }

        public string StatusMessage { get; set; }

    }
}
