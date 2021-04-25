using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMSWeb.Models
{
    public class LoginModels
    {
        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Language.Resource), ErrorMessageResourceName = "EmailLength")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Language.Resource), ErrorMessageResourceName = "EmailLength")]
        [RegularExpression("^[a-z0-9,!#$%&'*+/=?^_`{|}~-]+(.[a-z0-9,!#$%&'*+/=?^_`{|}~-]+)*@[a-z0-9-]+(.[a-z0-9-]+)*.([a-z]{2,})$", ErrorMessageResourceType = typeof(Language.Resource), ErrorMessageResourceName = "EmailRegEx")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class ForgotModels
    {
        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Language.Resource), ErrorMessageResourceName = "EmailLength")]
        [RegularExpression("^[a-z0-9,!#$%&'*+/=?^_`{|}~-]+(.[a-z0-9,!#$%&'*+/=?^_`{|}~-]+)*@[a-z0-9-]+(.[a-z0-9-]+)*.([a-z]{2,})$", ErrorMessageResourceType = typeof(Language.Resource), ErrorMessageResourceName = "EmailRegEx")]
        public string Email { get; set; }

        [Required]
        public string ReCaptchaParam { get; set; }
    }
}