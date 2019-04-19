﻿using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Users.Dto
{
    public class GetUsersToExcelInput: IShouldNormalize, IGetUsersInput
    {
        public string Filter { get; set; }

        public string Permission { get; set; }

        public int? Role { get; set; }

        public bool OnlyLockedUsers { get; set; }

        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Name,Surname";
            }
        }
    }
}
