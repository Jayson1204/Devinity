namespace LearningApp.Services
{
    public class ThemeService
    {
        public void SetTheme(AppTheme theme)
        {
            Application.Current.UserAppTheme = theme;
        }

        public AppTheme GetCurrentTheme()
        {
            return Application.Current.UserAppTheme;
        }

        public void ToggleTheme()
        {
            if (Application.Current.UserAppTheme == AppTheme.Dark)
                Application.Current.UserAppTheme = AppTheme.Light;
            else
                Application.Current.UserAppTheme = AppTheme.Dark;
        }

        public bool IsDark => Application.Current.UserAppTheme == AppTheme.Dark;
    }
}