using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace F1Dashboard
{
    class SteeringWheelLeds
    {
        public static float C_WIDTH = 450f;
        public static float C_HEIGHT = 36f;
        public static float RPM_MIN = 11030f;
        public static float RPM_MAX = 11830f;
        public static float RPM_MID = ((RPM_MAX - RPM_MIN) / 2) + RPM_MIN;
        public static int N_LEDS = 15;
        public static float RPM_STEP_SIZE = (RPM_MAX - RPM_MIN) / N_LEDS;
        public static float margin = 5f;
        public static float led_size = (C_WIDTH - ((margin * N_LEDS) + margin)) / N_LEDS;
        public static float led_offset = margin + led_size;
        private Canvas canvas;
        private Ellipse[] leds = new Ellipse[N_LEDS];
        private float[] RPM_STEPS = new float[N_LEDS];
        private float d_rpm = 0;

        public SteeringWheelLeds(Canvas _canvas)
        {
            this.canvas = _canvas;
        }

        public void Init()
        {
            for (int i = 0; i < N_LEDS; i++)
            {
                Ellipse elip = new Ellipse() {
                    Width = led_size,
                    Height = led_size,
                    Fill = ColorSet.DARK_GRAY
                };
                Canvas.SetTop(elip, margin);
                Canvas.SetLeft(elip, i * led_offset + margin);
                leds[i] = elip;
                RPM_STEPS[i] = i * RPM_STEP_SIZE + RPM_MIN;
                this.canvas.Children.Add(elip);
            }
            Rectangle rect = new Rectangle()
            {
                Width = C_WIDTH,
                Height = C_HEIGHT,
                Stroke = ColorSet.DARK_GRAY,
                StrokeThickness = 2
            };
            Canvas.SetTop(rect, 0);
            Canvas.SetLeft(rect, 0);
            canvas.Children.Add(rect);
        }

        public void Update(float rpm)
        {
            this.d_rpm = rpm;
            for (int i = 0; i < leds.Length; i++)
            {
                if (rpm > RPM_STEPS[i])
                {
                    if (i < 5)
                        this.leds[i].Fill = ColorSet.LED_GREEN;
                    else if (i < 10)
                        this.leds[i].Fill = ColorSet.LED_RED;
                    else
                        this.leds[i].Fill = ColorSet.LED_BLUE;
                    
                } else
                {
                    this.leds[i].Fill = ColorSet.DARK_GRAY;
                }
                if (rpm > RPM_MID)
                    this.canvas.Background = ColorSet.DARK_RED;
                else
                    this.canvas.Background = Brushes.Black;
            }
            
        }
    }
}
