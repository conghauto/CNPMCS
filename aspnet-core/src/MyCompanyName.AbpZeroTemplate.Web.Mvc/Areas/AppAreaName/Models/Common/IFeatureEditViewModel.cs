using System.Collections.Generic;
using Abp.Application.Services.Dto;
using MyCompanyName.AbpZeroTemplate.Editions.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.Areas.AppAreaName.Models.Common
{
    public interface IFeatureEditViewModel
    {
        List<NameValueDto> FeatureValues { get; set; }

        List<FlatFeatureDto> Features { get; set; }
    }
}