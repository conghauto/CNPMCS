using System.ComponentModel.DataAnnotations;

namespace MyCompanyName.AbpZeroTemplate.Friendships.Dto
{
    public class UnblockUserInput
    {
        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        public int? TenantId { get; set; }
    }
}