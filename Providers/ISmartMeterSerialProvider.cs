using System;
using SmartMeterToMqtt.Models;

namespace SmartMeterToMqtt.Providers
{
    public interface ISmartMeterSerialProvider : IDisposable
    {
        event EventHandler<P1Telegram> P1TelegramReceived;
        void Open(string portName, int baudRate);
        void Close();
    }
}