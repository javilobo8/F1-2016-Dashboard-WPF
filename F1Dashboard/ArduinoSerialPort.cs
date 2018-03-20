using System;
using System.IO.Ports;

namespace F1Dashboard
{
    class ArduinoSerialPort
    {
        public static string[] GetAvailablePorts() { return SerialPort.GetPortNames(); }
        public static int baudRate = 9600;

        public String Port { get; set; }
        private SerialPort sp;
        private bool isConnected = false;

        public ArduinoSerialPort() { }

        public void Init() {
            if (isConnected || Port == null) return;
            sp = new SerialPort(Port, baudRate);
            sp = new SerialPort("COM1", baudRate, Parity.None, 8, StopBits.One);
            sp.Open();
            isConnected = true;
        }

        public void End() {
            if (!isConnected) return;
            sp.Close();
            isConnected = false;
        }

        public void SendData(double rpm)
        {
            if (!isConnected) return;
            String message = String.Format("{0}", rpm);
            sp.Write(message);
        }
    }
}
