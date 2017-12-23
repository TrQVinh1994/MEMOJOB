using System.ComponentModel.DataAnnotations;

namespace MEMO_JOB.Models.JobSeekers.AccountViewModels
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
      
    }
}
