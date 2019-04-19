using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {
        [Required]
        [MaxLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}