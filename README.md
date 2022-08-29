# SmartMeterToMqtt
SmartMeterToMqtt is a simple application that sends Dutch smart meter data to a mqtt broker. A separate serial P1 cable is required to read the data from the smart meter.


## Run with Docker
Build and run SmartMeterToMqtt by cloning this repostory and executing the following commands:

```
git clone https://github.com/bfrerejean/SmartMeterToMqtt.git
cd SmartMeterToMqtt

docker build -t smartmetertomqtt .
docker run -d --name smartmetertomqtt --restart=always -e MQTTHOST=<hostname or ip-address> -e SERIALPORT=<serial portname> --device=<serial portname> smartmetertomqtt
```

Example:
```
docker run -d --name smartmetertomqtt --restart=always -e MQTTHOST=localhost -e SERIALPORT=/dev/ttyUSB0 --device=/dev/ttyUSB0 smartmetertomqtt
```

## Environment Variables

| Variable name  | Description | Required | Default value |
| ------------- | ------------- | ------------- | ------------- |
| MQTTHOST | Hostname of the MQTT broker | No | localhost |
| MQTTPORT | Port of the MQTT broker | No | 1883 |
| SERIALPORT | Name of the port to which the serial cable is connected  | No | /dev/ttyUSB0 |
| MQTTUSERNAME | Username to authenticate on the mqtt broker | No | |
| MQTTPASSWORD | Password to authenticate on the mqtt broker | No | |
| MQTTTLS | Use TLS | No | false |

## MQTT output
Once active, this application will publish the following message on the MQTT broker every few seconds (depending on the Smart Meter):
```
{
  "tariffIndicator": 2,
  "electricityCurrentlyDelivered": 1.234,
  "electricityCurrentlyReturned": 0,
  "electricityDeliveredLow": 1234.567,
  "electricityDeliveredHigh": 4567.890,
  "electricityReturnedLow": 123.456,
  "electricityReturnedHigh": 456.789,
  "electricityCurrentlyDeliveredL1": 0.345,
  "electricityCurrentlyDeliveredL2": 1.234,
  "electricityCurrentlyDeliveredL3": 0,
  "electricityCurrentlyReturnedL1": 0,
  "electricityCurrentlyReturnedL2": 0,
  "electricityCurrentlyReturnedL3": 0
}
```
Topic: ```smartmeter/p1telegram```
