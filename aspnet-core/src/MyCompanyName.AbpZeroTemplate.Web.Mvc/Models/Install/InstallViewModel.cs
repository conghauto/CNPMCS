using System.Collections.Generic;
using Abp.Localization;
using MyCompanyName.AbpZeroTemplate.Install.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.Models.Install
{
    public class InstallViewModel
    {
        public List<ApplicationLanguage> Languages { get; set; }

        public AppSettingsJsonDto AppSettingsJson { get; set; }
    }
}
