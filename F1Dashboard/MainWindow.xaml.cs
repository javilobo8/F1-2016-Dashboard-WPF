using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
namespace F1Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    delegate void ConvertMethod(byte[] packet);
    public partial class MainWindow : Window
    {
        private UDPServer server;
        private Thread listenThread;
        private ConvertMethod OnPacketReceivedMethod;
        private int speed_max = 0;
        private F1Data data;
        private F1Data prevData;
        private List<Lap> laps;
        private Brush whiteBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7FFFFFFF"));
        private SteeringWheelDisplay sw_display;
        private SteeringWheelLeds sw_leds;
        private int start_range = 90;
        private int end_range = 360 - 90;

        public MainWindow()
        {
            laps = new List<Lap>();
            OnPacketReceivedMethod = OnPacketReceived;
            server = new UDPServer(OnPacketReceivedMethod);
            listenThread = new Thread(() => server.listen());
            listenThread.Start();
            InitializeComponent();
            this.sw_display = new SteeringWheelDisplay(this.c_display);
            this.sw_leds = new SteeringWheelLeds(this.c_leds);
            this.UpdateSpeedLine(0);
            this.UpdateRpmLine(0);
            this.DrawSpeedSteps();
            this.DrawRpmSteps();
            this.sw_display.Init();
            this.sw_leds.Init();
        }

        private void DrawLine(Point p1, Point p2, Brush brush, int thickness)
        {
            Line line = new Line();
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            line.Stroke = brush;
            line.StrokeThickness = thickness;
            this.canvas.Children.Add(line);
        }

        private void DrawSpeedSteps()
        {
            Point center = new Point(100, 100);
            Point point1 = Util.Rotate(new Point(15, 100), center, -90);
            Point point2 = Util.Rotate(new Point(20, 100), center, -90);
            int step_unit = 10;
            float total_steps = 400 / step_unit;
            float distance = 270f;
            float step_deg_size = distance / total_steps;

            for (int i = 0; i <= total_steps; i++)
            {
                float deg = (i * step_deg_size);
                Point p1 = Util.Rotate(point1, center, deg);
                Point p2 = Util.Rotate(point2, center, deg);
                DrawLine(p1, p2, whiteBrush, 2);
            }
        }


        private void DrawRpmSteps()
        {
            Point center = new Point(100, 100);
            Point point1 = Util.Rotate(new Point(30, 100), center, -90);
            Point point2 = Util.Rotate(new Point(50, 100), center, -90);

            int step_unit = 1000;
            float total_steps = 14000 / step_unit;
            float distance = 270f;
            float step_deg_size = distance / total_steps;

            for (int i = 0; i <= total_steps; i++)
            {
                float deg = (i * step_deg_size);
                Point p1 = Util.Rotate(point1, center, deg);
                Point p2 = Util.Rotate(point2, center, deg);
                DrawLine(p1, p2, whiteBrush, 2);
            }
        }

        private void UpdateSpeedLine(double degrees)
        {
            Point point1 = new Point(11, 100);
            Point point2 = new Point(20, 100);
            Point center = new Point(100, 100);

            point1 = Util.Rotate(point1, center, degrees);
            point2 = Util.Rotate(point2, center, degrees);

            this.line_speed.X1 = point1.X;
            this.line_speed.Y1 = point1.Y;
            this.line_speed.X2 = point2.X;
            this.line_speed.Y2 = point2.Y;
        }


        private void UpdateRpmLine(double degrees)
        {
            Point point1 = new Point(30, 100);
            Point point2 = new Point(85, 100);
            Point center = new Point(100, 100);

            point1 = Util.Rotate(point1, center, degrees);
            point2 = Util.Rotate(point2, center, degrees);

            this.line_rpm.X1 = point1.X;
            this.line_rpm.Y1 = point1.Y;
            this.line_rpm.X2 = point2.X;
            this.line_rpm.Y2 = point2.Y;
        }

        private void OnPacketReceived(byte[] packet)
        {
            prevData = data;
            data = new F1Data(packet);
            // Calc
            float engineRate = data.Get("engineRate");
            float engineRatePercent = Util.GetPercent(engineRate, 14000);

            float speed = Util.MPHtoKMH(data.Get("speed"));
            float speedPercent = Util.GetPercent(speed, 100);

            float throttle = data.Get("throttle");
            float throttlePercent = Util.GetPercent(throttle, 1);

            float brake = data.Get("brake");
            float brakePercent = Util.GetPercent(brake, 1);

            // Update
            if (speed > speed_max)
            {
                this.speed_max = (int)speed;
            }
            if (prevData != null && data.Get("lap") != prevData.Get("lap"))
            {
                laps.Add(new Lap(prevData));
                foreach(Lap lap in laps)
                {
                    Console.WriteLine(lap.ToString());
                }
                Console.WriteLine("LAP: {0} LAP TIME: {1} LAP TIME: {2}", prevData.Get("lap"), prevData.Get("lapTime"), data.Get("last_lap_time"));
            }

            float speed_degrees = Util.CalcDegrees(speed, start_range, end_range, 400);
            float rpm_degrees = Util.CalcDegrees(engineRate, start_range, end_range, 14000);

            // Draw
            this.Dispatcher.Invoke(() =>
            { 
                this.lbl_speed_max.Content = String.Format("max: {0:N0} KM / h", speed_max); 
                this.lbl_time.Content = Formatter.Seconds(data.Get("time"));
                this.lbl_lapTime.Content = Formatter.Seconds(data.Get("lapTime"));
                this.lbl_sector1_time.Content = Formatter.Seconds(data.Get("sector1_time"));
                this.lbl_sector2_time.Content = Formatter.Seconds(data.Get("sector2_time"));
                this.lbl_last_lap_time.Content = Formatter.Seconds(data.Get("last_lap_time"));
                this.pbar_throttle.Value = throttlePercent;
                this.pbar_brake.Value = brakePercent;

                this.UpdateSpeedLine(speed_degrees);
                this.UpdateRpmLine(rpm_degrees);
                // this.dgrid_data.ItemsSource = LoadTableData(data);

                this.sw_leds.Update(engineRate);
                sw_display.Update(data);
                sw_display.Draw();
            });
            
        }
    }
}
