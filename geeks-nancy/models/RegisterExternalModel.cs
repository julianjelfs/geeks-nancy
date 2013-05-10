using System.ComponentModel.DataAnnotations;

namespace geeks_nancy.models
{
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        public string ExternalLoginData { get; set; }
        public string ReturnUrl { get; set; }
    }
}