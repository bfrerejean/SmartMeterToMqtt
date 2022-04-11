using System;
using System.IO.Ports;
using System.Threading.Tasks;
using SmartMeterToMqtt.Models;
using SmartMeterToMqtt.Providers;

namespace SmartMeterToMqtt.Infrastructure
{
    public class SmartMeterSerialProvider : ISmartMeterSerialProvider
    {
        private SerialPort _serialPort;

        public event EventHandler<P1Telegram> P1TelegramReceived;

        public void Open(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.Open();

            Task.Run(() => ReadMessages());
        }

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        private void ReadMessages()
        {
            string message = "";

            while (_serialPort.IsOpen)
            {
                string line = _serialPort.ReadLine();

                message += line + System.Environment.NewLine;

                if (line.StartsWith("!"))
                {
                    var p1Telegram = new P1Telegram(message);
                    message = "";

                    // Prevent empty messages
                    if (p1Telegram.ElectricityDeliveredHigh > 0)
                    {
                        P1TelegramReceived?.Invoke(this, p1Telegram);
                    }
                }
            }
        }

        public void Dispose()
        {
            _serialPort?.Dispose();
        }
    }
}