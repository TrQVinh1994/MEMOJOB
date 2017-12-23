using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MEMO_JOB.Models.JobSeekers.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [HiddenInput(DisplayValue = false)]
        [Required]
        [Display(Name = "Email", Prompt = "Nhập địa chỉ email")]
        [StringLength(250, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Sai định dạng email")]
        public string Email { get; set; }

        [HiddenInput(DisplayValue = false)]
        [Required]
        public string FullName { get; set; }
        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Mật khẩu", Prompt = "Mật khẩu")]
        //[StringLength(100, ErrorMessage = "{0} tối thiểu {2} ký tự không trùng nhau.", MinimumLength = 8)]
        //public string Password { get; set; }

    }
}
