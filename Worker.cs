using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SmartMeterToMqtt.Models;
using SmartMeterToMqtt.Providers;

namespace SmartMeterToMqtt
{
    public class Worker : BackgroundService
    {
        private IConfiguration            _configuration;
        private IMqttProvider             _mqttProvider;
        private ISmartMeterSerialProvider _smartMeterSerialProvider;

        public Worker(IConfiguration configuration, IMqttProvider mqttProvider, ISmartMeterSerialProvider smartMeterSerialProvider)
        {
            _configuration            = configuration;
            _mqttProvider             = mqttProvider;
            _smartMeterSerialProvider = smartMeterSerialProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string serialPort   = _configuration.GetValue<string>("SERIALPORT", "/dev/ttyUSB0");
            string mqttHost     = _configuration.GetValue<string>("MQTTHOST", "localhost");
            int mqttPort        = _configuration.GetValue<int>("MQTTPORT", 1883);
            string mqttUsername = _configuration.GetValue<string>("MQTTUSERNAME", null);
            string mqttPassword = _configuration.GetValue<string>("MQTTPASSWORD", null);
            bool mqttTls        = _configuration.GetValue<bool>("MQTTTLS", false);

            await _mqttProvider.ConnectAsync(mqttHost, mqttPort, mqttUsername, mqttPassword, mqttTls);

            _smartMeterSerialProvider.Open(serialPort, 115200);
            _smartMeterSerialProvider.P1TelegramReceived += SmartMeterSerialProvider_P1TelegramReceived;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _smartMeterSerialProvider.Close();
            await _mqttProvider.DisconnectAsync();

            _mqttProvider.Dispose();
            _smartMeterSerialProvider.Dispose();
        }

        private async void SmartMeterSerialProvider_P1TelegramReceived(object sender, P1Telegram e)
        {
            await _mqttProvider.PublishMessageAsync<P1Telegram>("smartmeter/p1telegram", e);
        }
    }
}