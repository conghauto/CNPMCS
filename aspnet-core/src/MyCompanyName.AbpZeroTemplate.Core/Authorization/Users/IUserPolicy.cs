﻿using System.Threading.Tasks;
using Abp.Domain.Policies;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}
