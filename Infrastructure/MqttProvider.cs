using System;
using System.Text.Json;
using System.Threading.Tasks;
using SmartMeterToMqtt.Providers;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace SmartMeterToMqtt.Infrastructure
{
    public class MqttProvider : IMqttProvider
    {
        MQTTnet.Client.IMqttClient _mqttClient;

        public async Task ConnectAsync(string uri,
            int port,
            string username = null,
            string password = null,
            bool tls = false)
        {
            string clientId = Guid.NewGuid().ToString();

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(uri, port)
                .WithCleanSession();

            if (username != null && password != null)
            {
                optionsBuilder.WithCredentials(username, password);
            }

            if (tls)
            {
                optionsBuilder.WithTls();
            }

            var options = optionsBuilder.Build();

            _mqttClient = new MqttFactory().CreateMqttClient();
            await _mqttClient.ConnectAsync(options);
        }

        public async Task DisconnectAsync()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
        }

        public async Task PublishMessageAsync<T>(string topic, T message)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string payload = JsonSerializer.Serialize<T>(message, serializeOptions);
            await _mqttClient.PublishAsync(topic, payload);
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
        }
    }
}