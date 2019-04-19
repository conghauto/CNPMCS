using Abp.AutoMapper;
using MyCompanyName.AbpZeroTemplate.Organizations.Dto;

namespace MyCompanyName.AbpZeroTemplate.Models.Users
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}