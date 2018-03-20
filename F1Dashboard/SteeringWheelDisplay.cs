using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace F1Dashboard
{
    class SteeringWheelDisplay
    {
        public static int C_WIDTH = 450;
        public static float C_HEIGHT = 250;
        public static int FONT_BIG = 80;
        public static Brush WHITE = Brushes.White;

        private Canvas c_display;

        private String d_gear = "N";
        private int d_kmh = 0;
        private int d_rpm = 0;
        private String d_lap_time = "00:00.000";

        private TextBlock txt_gear = new TextBlock();
        private TextBlock txt_lap_time = new TextBlock();

        public SteeringWheelDisplay(Canvas _c_display)
        {
            c_display = _c_display;
        }

        public void Init()
        {
            c_display.Width = C_WIDTH;
            c_display.Height = C_HEIGHT;
            c_display.Background = Brushes.Black;

            // Lines
            Rectangle rect = new Rectangle()
            {
                Width = C_WIDTH,
                Height = C_HEIGHT,
                Stroke = ColorSet.DARK_GRAY,
                StrokeThickness = 2
            };
            Canvas.SetTop(rect, 0);
            Canvas.SetLeft(rect, 0);
            c_display.Children.Add(rect);
            c_display.Children.Add(new Line()
            {
                X1 = 0,
                Y1 = C_HEIGHT / 2 + 30,
                X2 = C_WIDTH,
                Y2 = C_HEIGHT / 2 + 30,
                Stroke = ColorSet.DARK_GRAY,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            });
            c_display.Children.Add(new Line()
            {
                X1 = C_WIDTH / 2 - 40,
                Y1 = 0,
                X2 = C_WIDTH / 2 - 40,
                Y2 = C_HEIGHT / 2 + 30,
                Stroke = ColorSet.DARK_GRAY,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            });
            c_display.Children.Add(new Line()
            {
                X1 = C_WIDTH / 2 + 40,
                Y1 = 0,
                X2 = C_WIDTH / 2 + 40,
                Y2 = C_HEIGHT / 2 + 30,
                Stroke = ColorSet.DARK_GRAY,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            });
            // Gear text
            txt_gear.FontWeight = FontWeights.Bold;
            txt_gear.Foreground = WHITE;
            txt_gear.FontSize = FONT_BIG;

            c_display.Children.Add(txt_gear);

            TextBlock txt_gear_lbl = new TextBlock()
            {
                Text = "Gear",
                FontSize = 16,
                Foreground = WHITE
            };
            Canvas.SetLeft(txt_gear_lbl, C_WIDTH / 2 - (txt_gear_lbl.ActualWidth / 2) - 15);
            Canvas.SetTop(txt_gear_lbl, C_HEIGHT / 2 - (txt_gear_lbl.ActualHeight / 2) + 4);
            c_display.Children.Add(txt_gear_lbl);

            txt_lap_time = new TextBlock()
            {
                Text = d_lap_time,
                FontSize = 30,
                Foreground = WHITE
            };
            Canvas.SetLeft(txt_lap_time, 10);
            Canvas.SetTop(txt_lap_time, 5);
            c_display.Children.Add(txt_lap_time);

            Draw();
        }

        public void Update(F1Data data)
        {
            this.d_gear = Constants.GEARS[(int)data.Get("gear")];
            this.d_lap_time = Formatter.Seconds(data.Get("lapTime"));
        }

        public void Draw()
        {
            txt_gear.Text = d_gear;
            txt_gear.Measure(new Size(0, 0));
            txt_gear.Arrange(new Rect());
            Canvas.SetLeft(txt_gear, C_WIDTH / 2 - (txt_gear.ActualWidth / 2));
            Canvas.SetTop(txt_gear, C_HEIGHT / 2 - (txt_gear.ActualHeight / 2) - 35);

            txt_lap_time.Text = this.d_lap_time;
        }
    }
}
