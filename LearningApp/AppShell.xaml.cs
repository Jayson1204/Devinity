using LearningApp.Views;

namespace LearningApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("CourseDetailPage", typeof(CourseDetailPage));
            Routing.RegisterRoute("CodeEditorPage", typeof(CodeEditorPage));
            Routing.RegisterRoute("CertificatePage", typeof(CertificatePage));
        }
    }
}