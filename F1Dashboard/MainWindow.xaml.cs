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

        private Speedometer speedometer;
        private SteeringWheelDisplay sw_display;
        private SteeringWheelLeds sw_leds;

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
            this.speedometer = new Speedometer(this.c_speedometer);


            this.sw_display.Init();
            this.sw_leds.Init();
            this.speedometer.Init();
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

                sw_leds.Update(engineRate);
                sw_display.Update(data);
                speedometer.Update(data);

                sw_display.Draw();


            });
            
        }
    }
}
