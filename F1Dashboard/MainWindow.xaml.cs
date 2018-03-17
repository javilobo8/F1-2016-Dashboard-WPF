using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow()
        {
            laps = new List<Lap>();
            OnPacketReceivedMethod = OnPacketReceived;
            server = new UDPServer(OnPacketReceivedMethod);
            listenThread = new Thread(() => server.listen());
            listenThread.Start();
            InitializeComponent();
            this.UpdateSpeedLine(-45);
            this.UpdateRpmLine(-45);
        }

        private double degToRad(double angle)
        {
            return Math.PI / 180 * angle;
        }

        private Point rotate(Point point, Point center, double degrees)
        {
            double angle = degToRad(degrees);
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            double new_p1x = (cos * (point.X - center.X)) - (sin * (point.Y - center.Y)) + center.Y;
            double new_p1y = (sin * (point.X - center.X)) + (cos * (point.Y - center.Y)) + center.Y;

            return new Point(new_p1x, new_p1y);
        }

        private void UpdateSpeedLine(double degrees)
        {
            Point point = new Point(10, 100);
            Point center = new Point(100, 100);

            point = rotate(point, center, degrees);
            
            this.line_speed.X1 = point.X;
            this.line_speed.Y1 = point.Y;
            this.line_speed.X2 = center.X;
            this.line_speed.Y2 = center.Y;
        }


        private void UpdateRpmLine(double degrees)
        {
            Point point = new Point(30, 100);
            Point center = new Point(100, 100);

            point = rotate(point, center, degrees);

            this.line_rpm.X1 = point.X;
            this.line_rpm.Y1 = point.Y;
            this.line_rpm.X2 = center.X;
            this.line_rpm.Y2 = center.Y;
        }

        private void OnPacketReceived(byte[] packet)
        {
            prevData = data;
            data = new F1Data(packet);
            // Calc
            float engineRate = (int)data.Get("engineRate");
            float engineRatePercent = engineRate * 100 / 14000;

            float speed = (int)(data.Get("speed") * 3.6);
            float speedPercent = speed * 100 / 400;

            float throttle = data.Get("throttle");
            float throttlePercent = throttle * 100 / 1;
            float brake = data.Get("brake");
            float brakePercent = brake * 100 / 1;

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

            int start_range = +45;
            int end_range = 360 - 90;

            float speed_degrees = (speed * end_range / 400) - start_range;
            float rpm_degrees = (engineRate * end_range / 14000) - start_range;

            // Draw
            this.Dispatcher.Invoke(() =>
            { 
                this.lbl_rpm.Content = String.Format("{0} RPM", engineRate, engineRatePercent);
                this.lbl_speed.Content = String.Format("{0} KM/h", speed);
                this.lbl_speed_max.Content = String.Format("max: {0} KM / h", speed_max); 
                this.lbl_time.Content = Formatter.Seconds(data.Get("time"));
                this.lbl_lapTime.Content = Formatter.Seconds(data.Get("lapTime"));
                this.lbl_sector1_time.Content = Formatter.Seconds(data.Get("sector1_time"));
                this.lbl_sector2_time.Content = Formatter.Seconds(data.Get("sector2_time"));
                this.lbl_last_lap_time.Content = Formatter.Seconds(data.Get("last_lap_time"));
                this.pbar_rpm.Value = engineRatePercent;
                this.pbar_speed.Value = speedPercent;
                this.pbar_throttle.Value = throttlePercent;
                this.pbar_brake.Value = brakePercent;
                this.lbl_gear.Content = Constants.GEARS[(int)data.Get("gear")];

                this.lbl_wheel_speed_bl.Content = data.Get("wheel_speed_bl");
                this.lbl_wheel_speed_br.Content = data.Get("wheel_speed_br");
                this.lbl_wheel_speed_fl.Content = data.Get("wheel_speed_fl");
                this.lbl_wheel_speed_fr.Content = data.Get("wheel_speed_fr");

                this.UpdateSpeedLine(speed_degrees);
                this.UpdateRpmLine(rpm_degrees);
                // this.dgrid_data.ItemsSource = LoadTableData(data);
            });
            
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdateSpeedLine(this.slider.Value);
        }
    }
}
