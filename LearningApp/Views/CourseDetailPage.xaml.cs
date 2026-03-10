using LearningApp.Models;
using LearningApp.ViewModels;

namespace LearningApp.Views
{
    public partial class CourseDetailPage : ContentPage
    {
        private readonly CourseDetailViewModel _viewModel;
        private readonly string _courseName;
        private bool _needsRefresh = false;

        public CourseDetailPage(string courseName)
        {
            InitializeComponent();
            _courseName = courseName;
            CourseTitleLabel.Text = courseName;
            _viewModel = new CourseDetailViewModel(
                courseName,
                onVideoTapped: OnVideoTapped,
                onAssessmentTapped: OnAssessmentTapped);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SkeletonScroll.IsVisible = true;
            CourseCollectionView.IsVisible = false;

            Dispatcher.Dispatch(async () =>
            {
                await _viewModel.LoadAsync(forceRefresh: _needsRefresh);
                _needsRefresh = false;

                SkeletonScroll.IsVisible = false;
                CourseCollectionView.IsVisible = true;
            });
        }

        private async void OnVideoTapped(VideoItem video)
        {
            if (string.IsNullOrEmpty(video.FirebaseUrl))
            {
                await DisplayAlert("Error", "Video URL is missing.", "OK");
                return;
            }
            await _viewModel.MarkVideoWatched(video.Id, video.Category);
            await Navigation.PushModalAsync(
                new VideoPlayerPage(video.FirebaseUrl, video.Title, video.Duration ?? ""));
        }

        private async void OnAssessmentTapped(AssessmentItem assessment)
        {
            var a = new Assessment
            {
                Id = assessment.Id.ToString(),
                Title = assessment.Title,
                Level = assessment.Level,
                Challenges = new List<CodeChallenge>
                {
                    new CodeChallenge
                    {
                        Id             = assessment.Id.ToString(),
                        Question       = assessment.Question,
                        StarterCode    = assessment.StarterCode,
                        ExpectedOutput = assessment.ExpectedOutput
                    }
                }
            };

            _viewModel.InvalidateCache();
            _needsRefresh = true;
            await Navigation.PushAsync(new CodeEditorPage(a, _courseName, assessment.Id));
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}