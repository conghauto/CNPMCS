using System.ComponentModel.DataAnnotations;

namespace MyCompanyName.AbpZeroTemplate.Web.Models.Account
{
    public class SendPasswordResetLinkViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}