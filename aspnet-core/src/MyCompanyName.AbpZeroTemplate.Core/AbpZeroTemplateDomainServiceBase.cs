﻿using Abp.Domain.Services;

namespace MyCompanyName.AbpZeroTemplate
{
    public abstract class AbpZeroTemplateDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected AbpZeroTemplateDomainServiceBase()
        {
            LocalizationSourceName = AbpZeroTemplateConsts.LocalizationSourceName;
        }
    }
}
