using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SmartMeterToMqtt.Models
{
    public class P1Telegram
    {
        private Dictionary<string, string> _fields;

        public int TariffIndicator => GetValue<int>("0-0:96.14.0");
        public decimal ElectricityCurrentlyDelivered => GetValue<decimal>("1-0:1.7.0");
        public decimal ElectricityCurrentlyReturned => GetValue<decimal>("1-0:2.7.0");
        public decimal ElectricityDeliveredLow => GetValue<decimal>("1-0:1.8.1");
        public decimal ElectricityDeliveredHigh => GetValue<decimal>("1-0:1.8.2");
        public decimal ElectricityReturnedLow => GetValue<decimal>("1-0:2.8.1");
        public decimal ElectricityReturnedHigh => GetValue<decimal>("1-0:2.8.2");

        public decimal ElectricityCurrentlyDeliveredL1 => GetValue<decimal>("1-0:21.7.0");
        public decimal ElectricityCurrentlyDeliveredL2 => GetValue<decimal>("1-0:41.7.0");
        public decimal ElectricityCurrentlyDeliveredL3 => GetValue<decimal>("1-0:61.7.0");

        public decimal ElectricityCurrentlyReturnedL1 => GetValue<decimal>("1-0:22.7.0");
        public decimal ElectricityCurrentlyReturnedL2 => GetValue<decimal>("1-0:42.7.0");
        public decimal ElectricityCurrentlyReturnedL3 => GetValue<decimal>("1-0:62.7.0");
        
        public P1Telegram(string message)
        {
            _fields = new Dictionary<string, string>();

            foreach (var line in message.Split("\n"))
            {
                ParseLine(line);
            }
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