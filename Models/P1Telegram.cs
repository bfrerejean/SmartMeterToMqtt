using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SmartMeterToMqtt.Models
{
    public class P1Telegram
    {
        private Dictionary<string, string> _fields;

        public int TariffIndicator { get; }
        public decimal ElectricityCurrentlyDelivered { get; }
        public decimal ElectricityCurrentlyReturned { get; }
        public decimal ElectricityDeliveredLow { get; }
        public decimal ElectricityDeliveredHigh { get; }
        public decimal ElectricityReturnedLow { get; }
        public decimal ElectricityReturnedHigh { get; }
        public decimal ElectricityCurrentlyDeliveredL1 { get; }
        public decimal ElectricityCurrentlyDeliveredL2 { get; }
        public decimal ElectricityCurrentlyDeliveredL3 { get; }
        public decimal ElectricityCurrentlyReturnedL1 { get; }
        public decimal ElectricityCurrentlyReturnedL2 { get; }
        public decimal ElectricityCurrentlyReturnedL3 { get; }

        public P1Telegram(string message)
        {
            _fields = new Dictionary<string, string>();

            foreach (var line in message.Split("\n"))
            {
                ParseLine(line);
            }

            TariffIndicator = GetValue<int>("0-0:96.14.0");
            ElectricityCurrentlyDelivered = GetValue<decimal>("1-0:1.7.0");
            ElectricityCurrentlyReturned = GetValue<decimal>("1-0:2.7.0");
            ElectricityDeliveredLow = GetValue<decimal>("1-0:1.8.1");
            ElectricityDeliveredHigh = GetValue<decimal>("1-0:1.8.2");
            ElectricityReturnedLow = GetValue<decimal>("1-0:2.8.1");
            ElectricityReturnedHigh = GetValue<decimal>("1-0:2.8.2");
            ElectricityCurrentlyDeliveredL1 = GetValue<decimal>("1-0:21.7.0");
            ElectricityCurrentlyDeliveredL2 = GetValue<decimal>("1-0:41.7.0");
            ElectricityCurrentlyDeliveredL3 = GetValue<decimal>("1-0:61.7.0");
            ElectricityCurrentlyReturnedL1 = GetValue<decimal>("1-0:22.7.0");
            ElectricityCurrentlyReturnedL2 = GetValue<decimal>("1-0:42.7.0");
            ElectricityCurrentlyReturnedL3 = GetValue<decimal>("1-0:62.7.0");
        }

        private void ParseLine(string line)
        {
            var regex = new Regex(@"(?<key>^.*?(?=\())((?:\((?<value>[^)]*)\*)|(?:\((?<value>[^)]*)\)))");
            var match = regex.Match(line);

            if (match.Success)
            {
                _fields.Add(match.Groups["key"].Value, match.Groups["value"].Value);
            }
        }

        private T GetValue<T>(string keyName)
        {
            var value = _fields.GetValueOrDefault(keyName);

            if (value != null)
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return default(T);
        }
    }
}