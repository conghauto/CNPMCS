﻿using Abp.Events.Bus;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy
{
    public class RecurringPaymentsEnabledEventData : EventData
    {
        public int TenantId { get; set; }
    }
}