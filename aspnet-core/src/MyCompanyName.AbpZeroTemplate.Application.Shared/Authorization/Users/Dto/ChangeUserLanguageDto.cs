using System.ComponentModel.DataAnnotations;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
