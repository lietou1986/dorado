namespace Dorado.Platform.Themes
{
    public class ThemeSwitchedEvent
    {
        public string OldTheme { get; set; }
        public string NewTheme { get; set; }
        public bool IsMobile { get; set; }
    }
}