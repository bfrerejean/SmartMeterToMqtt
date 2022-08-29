using System;
using System.IO.Ports;
using SmartMeterToMqtt.Models;
using SmartMeterToMqtt.Providers;

namespace SmartMeterToMqtt.Infrastructure
{
    public class SmartMeterSerialProvider : ISmartMeterSerialProvider
    {
        private SerialPort _serialPort;
        private string _messageBuffer;

        public event EventHandler<P1Telegram> P1TelegramReceived;

        public void Open(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.Open();
            _serialPort.RtsEnable = true;

            _serialPort.DataReceived += (sender, e) =>
            {
                _messageBuffer += _serialPort.ReadExisting();

                if (_messageBuffer.Contains("!"))
                {
                    try
                    {
                        var p1Telegram = new P1Telegram(_messageBuffer);

                        // Prevent empty messages
                        if (p1Telegram.ElectricityDeliveredHigh > 0)
                        {
                            P1TelegramReceived?.Invoke(this, p1Telegram);
                        }

                        System.Threading.Thread.Sleep(1000);
                    }
                    catch (FormatException ex)
                    {
                        // Error while processing telegram message, skip message.
                        Console.Error.WriteLine("Error while processing telegram message. {0}", ex.Message);
                    }
                    finally
                    {
                        _messageBuffer = "";
                    }
                }
            };
        }

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void Dispose()
        {
            _serialPort?.Dispose();
        }
    }
}