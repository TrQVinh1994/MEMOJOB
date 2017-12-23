using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MEMO_JOB.Models.Recruiters.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu", Prompt = "Mật khẩu")]
        [StringLength(100, ErrorMessage = "{0} tối thiểu {2} ký tự không trùng nhau.", MinimumLength = 8)]
        public string Password { get; set; }

    }
}
