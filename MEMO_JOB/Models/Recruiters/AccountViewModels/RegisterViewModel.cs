using System;
using System.ComponentModel.DataAnnotations;

namespace MEMO_JOB.Models.Recruiters.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Họ tên", Prompt = "Họ và tên")]
        [StringLength(250, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]       
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Email", Prompt = "Nhập địa chỉ email")]
        [StringLength(250, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Sai định dạng email")]
        //[RegularExpression(@"/^(([^<>()\[\]\.,;:\s@\']+(\.[^<>()\[\]\.,;:\s@\']+)*)|(\'.+\'))@(([^<>()[\]\.,;:\s@\']+\.)+[^<>()[\]\.,;:\s@\']{2,})$", ErrorMessage = "Sai định dạng email")]        
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Điện thoại", Prompt = "Nhập số điện thoại")]
        [StringLength(100, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]       
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu", Prompt = "Mật khẩu")]
        [StringLength(100, ErrorMessage = "{0} tối thiểu {2} ký tự không trùng nhau.", MinimumLength = 1)] 
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name="Nhập lại mật khẩu", Prompt = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Nhập lại mật khẩu không đúng.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Fax", Prompt = "Fax")]
        public string Fax { get; set; }
        [Display(Name = "Chức vụ", Prompt = "Chức vụ")]
        public string Position { get; set; }
      
        [Display(Name = "Điện thoại cố định", Prompt = "Nhập số điện thoại bàn")]
        public string PhoneDesk { get; set; }

        [Display(Name = "Tên công ty", Prompt = "Tên công ty")]
        public string CompanyName { get; set; }
        [Display(Name = "Quy mô công ty", Prompt = "ví dụ 5->20 nhân viên")]
        public string CompanySize { get; set; }
        [Display(Name = "Thông tin công ty", Prompt = "Nhập thông tin sơ lược về công ty của bạn")]
        public string CompanyInfo { get; set; }

        [Display(Name = "Địa chỉ", Prompt = "Nhập địa chỉ liên hệ")]
        public string Address { set; get; }
        [Display(Name = "Thành phố", Prompt = "Thành phố")]
        public string City { set; get; }
        [Display(Name = "Quốc gia", Prompt = "Quốc gia")]
        public string Nation { set; get; }
    }
}
