namespace LearningApp.Controls
{
    public class SkeletonView : ContentView
    {
        private bool _isAnimating = false;

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(SkeletonView), 8f,
                propertyChanged: (b, o, n) => ((SkeletonView)b).UpdateShape());

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public SkeletonView()
        {
            BackgroundColor = Color.FromArgb("#E0E0E0");
            UpdateShape();
        }

        private void UpdateShape()
        {
            var border = new Border
            {
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(CornerRadius)
                }
            };
            Content = border;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent != null)
                StartAnimation();
            else
                _isAnimating = false;
        }

        private async void StartAnimation()
        {
            _isAnimating = true;
            while (_isAnimating && Parent != null)
            {
                await this.FadeTo(0.4, 700, Easing.SinInOut);
                if (!_isAnimating) break;
                await this.FadeTo(1.0, 700, Easing.SinInOut);
            }
        }
    }
}