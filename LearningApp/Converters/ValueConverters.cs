using System.Globalization;

namespace LearningApp.Converters
{
    // Converts string to bool (true if not empty)
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converts string to border color (red if error exists, gray otherwise)
    public class StringToBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Color.FromArgb("#E5E7EB") : Color.FromArgb("#F87171");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Inverts boolean value
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    // Converts bool to eye icon (for password visibility toggle)
    public class PasswordIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Option 1: Use emoji (works immediately, no images needed)
            return (bool)value ? "🙈" : "👁️";

            // Option 2: If you prefer image icons, uncomment below and add eye.png/eye_off.png to Resources/Images
            // return (bool)value ? "eye_off.png" : "eye.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}