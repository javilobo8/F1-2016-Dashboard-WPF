using System;
using System.Collections.Generic;
using System.Windows;

namespace F1Dashboard
{
    delegate void ConvertMethod(byte[] packet);

    public partial class MainWindow : Window
    {
        private ArduinoSerialPort arduino;
        private UDPServer server;

        private ConvertMethod OnPacketReceivedMethod;
        private int speed_max = 0;
        private F1Data data;
        private F1Data prevData;
        private List<Lap> laps = new List<Lap>();

        private Speedometer speedometer;
        private SteeringWheelDisplay sw_display;
        private SteeringWheelLeds sw_leds;

        public MainWindow()
        {
            OnPacketReceivedMethod = OnPacketReceived;
            server = new UDPServer(OnPacketReceivedMethod);
            arduino = new ArduinoSerialPort();
            InitializeComponent();

            sw_display = new SteeringWheelDisplay(c_display);
            sw_leds = new SteeringWheelLeds(c_leds);
            speedometer = new Speedometer(c_speedometer);
            cmb_port.DisplayMemberPath = "Key";
            cmb_port.SelectedValuePath = "Value";

            // Canvas Init
            sw_display.Init();
            sw_leds.Init();
            speedometer.Init();

            SetAvailablePorts();
            server.Listen();
        }

        public async void SetAvailablePorts()
        {
            string[] ports = ArduinoSerialPort.GetAvailablePorts();

            cmb_port.Items.Clear();
            foreach (string port in ports)
            {
                cmb_port.Items.Add(new KeyValuePair<string, string>(port, port));
            }
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
            if (speed > speed_max) speed_max = (int)speed;
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
                lbl_speed_max.Content = String.Format("max: {0:N0} KM / h", speed_max); 
                lbl_time.Content = Formatter.Seconds(data.Get("time"));
                lbl_lapTime.Content = Formatter.Seconds(data.Get("lapTime"));
                lbl_sector1_time.Content = Formatter.Seconds(data.Get("sector1_time"));
                lbl_sector2_time.Content = Formatter.Seconds(data.Get("sector2_time"));
                lbl_last_lap_time.Content = Formatter.Seconds(data.Get("last_lap_time"));
                pbar_throttle.Value = throttlePercent;
                pbar_brake.Value = brakePercent;

                sw_leds.Update(engineRate);
                sw_display.Update(data);
                speedometer.Update(data);

                sw_display.Draw();
            });
            
        }

        private void btn_arduino_connect_Click(object sender, RoutedEventArgs e)
        {
            arduino.Port = (string)cmb_port.SelectedValue;
            Console.WriteLine("PORT: {0}", arduino.Port);
            arduino.Init();
        }
    }
}
