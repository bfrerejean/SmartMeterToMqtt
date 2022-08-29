using System;
using System.Threading.Tasks;

namespace SmartMeterToMqtt.Providers
{
    public interface IMqttProvider : IDisposable
    {
        Task ConnectAsync(string uri, int port, string username = null, string password = null, bool tls = false);
        Task DisconnectAsync();

        Task PublishMessageAsync<T>(string topic, T message);
    }
}