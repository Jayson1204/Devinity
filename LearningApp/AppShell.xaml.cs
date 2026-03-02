using LearningApp.Views;

namespace LearningApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("CourseDetailPage", typeof(CourseDetailPage));
            Routing.RegisterRoute("CodeEditorPage", typeof(CodeEditorPage));
            Routing.RegisterRoute("MyLearningPage", typeof(MyLearningPage));
            Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
        }
    }
}