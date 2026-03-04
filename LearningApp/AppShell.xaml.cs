using LearningApp.Views;

namespace LearningApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Only register non-tab routes here
            // Tab routes (MainPage, MyLearningPage, ProfilePage) are auto-registered by TabBar
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("CourseDetailPage", typeof(CourseDetailPage));
            Routing.RegisterRoute("CodeEditorPage", typeof(CodeEditorPage));
        }
    }
}