using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.HostDashboard.Dto;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}