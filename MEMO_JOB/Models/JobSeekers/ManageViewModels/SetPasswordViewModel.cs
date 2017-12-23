using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MEMO_JOB.Models.JobSeekers.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required]       
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới", Prompt = "Mật khẩu mới")]
        [StringLength(100, ErrorMessage = "{0} tối thiểu {2} ký tự không trùng nhau.", MinimumLength = 8)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu mới", Prompt = "Mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Nhập lại mật khẩu không đúng.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
