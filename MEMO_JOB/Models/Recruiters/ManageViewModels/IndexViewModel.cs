using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MEMO_JOB.Models.Recruiters.ManageViewModels
{
    public class IndexViewModel
    {
        public string UserId { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]
        [Display(Name = "Họ và tên", Prompt = "Họ và tên")]
        public string FullName { get; set; }

        public string Address { get; set; }

        [Display(Name = "Thành phố", Prompt = "Thành phố")]
        [StringLength(250, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]
        public string City { get; set; }

        [Display(Name = "Quốc tịch")]
        [StringLength(250, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]
        public string Nation { get; set; }

        public bool Subscribe { get; set; }
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [Display(Name = "Email", Prompt = "Nhập địa chỉ email")]
        [StringLength(250, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Sai định dạng email")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Điện thoại", Prompt = "Nhập số điện thoại")]
        [StringLength(100, ErrorMessage = "{0} tối đa {1} ký tự.", MinimumLength = 1)]
        public string PhoneNumber { get; set; }

        public string PhoneDesk { get; set; }
        public string Fax { get; set; }
        public string Position { get; set; }
        public string StatusMessage { get; set; }
        public bool Active { get; set; }
    }
}
