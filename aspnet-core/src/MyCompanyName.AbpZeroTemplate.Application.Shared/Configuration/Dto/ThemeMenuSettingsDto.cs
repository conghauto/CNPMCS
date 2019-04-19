namespace MyCompanyName.AbpZeroTemplate.Configuration.Dto
{
    public class ThemeMenuSettingsDto
    {
        public string Position { get; set; }

        public string AsideSkin { get; set; }

        public bool FixedAside { get; set; }

        public bool AllowAsideMinimizing { get; set; }

        public bool DefaultMinimizedAside { get; set; }

        public bool AllowAsideHiding { get; set; }

        public bool DefaultHiddenAside { get; set; }

        public string SubmenuToggle { get; set; }
    }
}