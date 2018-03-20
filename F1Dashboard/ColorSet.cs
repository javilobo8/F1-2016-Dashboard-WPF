using System.Windows.Media;

namespace F1Dashboard
{
    class ColorSet
    {
        public static Brush DARK_GRAY = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3A3A3A"));
        public static Brush MD_WHITE = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7FFFFFFF"));
        public static Brush SM_GREEN = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF04DC53"));
        public static Brush SM_ORANGE = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDC6604"));
        public static Brush DARK_RED = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#420000"));
        public static Brush LED_RED = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d80a0a"));
        public static Brush LED_BLUE = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0971d8"));
        public static Brush LED_GREEN = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#08d832"));
    }
}
