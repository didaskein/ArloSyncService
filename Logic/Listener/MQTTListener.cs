using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Common.Extensions;
using ArloSyncService.Logic.ArloStation;
using ArloSyncService.Logic.ArloStation.Model;
using BF.Services.Logic;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Listener
{
    public class MQTTListener(ArloConfiguration ArloConfiguration, DeviceActivities DeviceActivities, ILogger<MQTTListener> Logger)
    {
        public bool isRunning { get; set; }


        public async Task StartMQTTListenRequestAsync(string token, string userId, string mqttUrl, List<string> mqttChannels, CancellationToken cancellationToken)
        {
            (string host, int port) = GetMqttServer(mqttUrl);

            var options = new HiveMQClientOptions
            {
                Host = ArloConfiguration.MQTTHostName,  //host
                Port = ArloConfiguration.MQTTPort,   //  we are using 8883 for SSL, api return another port 8084...
                UseTLS = true,
                KeepAlive = 60,
                ClientId = userId.GetRandomIdForUser(),
                UserName = userId,
                Password = token,
            };

            options.UserProperties = new Dictionary<string, string>
            {
                { "Auth-Version", "2" },
                { "Authorization", token },
                { "Host", ArloConfiguration.MQTTHostName },
                { "Origin", "https://my.arlo.com" }
            };

            var client = new HiveMQClient(options);
            Console.WriteLine($"Connecting to {options.Host} on port {options.Port} ...");

            // Connect
            HiveMQtt.Client.Results.ConnectResult connectResult;
            try
            {
                connectResult = await client.ConnectAsync().ConfigureAwait(false);
                if (connectResult.ReasonCode == HiveMQtt.MQTT5.ReasonCodes.ConnAckReasonCode.Success)
                {
                    Console.WriteLine($"Connect successful: {connectResult}");
                    isRunning = true;
                }
                else
                {
                    Console.WriteLine($"Connect failed: {connectResult.ToString()}");
                    isRunning = false;
                    Environment.Exit(-1);
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine($"Error connecting to the MQTT Broker with the following socket error: {e.Message}");
                isRunning = false;
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to the MQTT Broker with the following message: {e.Message}");
                isRunning = false;
                Environment.Exit(-1);
            }

            // Message Handler
            client.OnMessageReceived += (sender, args) =>
            {
                string received_message = args.PublishMessage.PayloadAsString;

                Console.WriteLine($"{DateTime.Now.ToString()} MQTT Topic={args.PublishMessage.Topic}");
                Console.WriteLine(received_message);
                Console.WriteLine("__________________________________________________________________________________________");

                if (received_message.Contains("port") && received_message.Contains("privateIP"))
                {
                    try
                    {
                        var stationInfo = JsonSerializer.Deserialize<ArloStationOpenPortAnswser>(received_message);
                        if (stationInfo != null && stationInfo.properties != null && !string.IsNullOrWhiteSpace(stationInfo?.properties?.privateIP))
                        {
                            DeviceActivities.UpdatePortIpStation(stationInfo.from, stationInfo.properties.privateIP, stationInfo.properties.port);
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "{Message}", ex.Message);
                    }

                }
                else if (received_message.Contains("USB") && received_message.Contains("FAT"))
                {
                    DeviceEvent deviceEvent = new DeviceEvent();
                    deviceEvent.Message = received_message; //Message in mqtt 

                    try
                    {
                        var stationRecord = JsonSerializer.Deserialize<ArloStationNewRecordAnswser>(received_message);
                        if (stationRecord != null)
                        {
                            deviceEvent.ParentId = stationRecord.from; //Id of the station "A7332677D0941"
                            deviceEvent.DeviceId = stationRecord.resource; //Id of the camera  "cameras/AB23267ND1061"
                            deviceEvent.Record = stationRecord;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "{Message}", ex.Message);
                    }

                    DeviceActivities.EnqueueDeviceEvent(deviceEvent);
                }

            };

            // Subscribe
            SubscribeOptions optionsSub = new SubscribeOptions();
            optionsSub.TopicFilters = new List<TopicFilter>
            {
                new TopicFilter("u/" + userId + "/#"),
                new TopicFilter("u/" + userId + "/in/userSession/connect"),
                new TopicFilter("u/" + userId + "/in/userSession/disconnect"),
                new TopicFilter("u/" + userId + "/in/library/add"),
                new TopicFilter("u/" + userId + "/in/library/update"),
                new TopicFilter("u/" + userId + "/in/library/remove"),

            };

            //Add mqtt channels of all devices
            foreach (var mqttChannel in mqttChannels)
            {
                optionsSub.TopicFilters.Add(new TopicFilter(mqttChannel));
            }


            await client.SubscribeAsync(optionsSub);

            do
            {
                await Task.Delay(5000);
            }
            while (!cancellationToken.IsCancellationRequested);

        }

        /// <summary>
        /// Extract Host & Port
        /// </summary>
        /// <param name="mqttUrl">"wss://mqtt-cluster-z1-1.arloxcld.com:8084"</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private (string host, int port) GetMqttServer(string mqttUrl)
        {
            Regex regex = new Regex(@"wss://([^:/]+):(\d+)");
            Match match = regex.Match(mqttUrl);

            if (match.Success)
            {
                string host = match.Groups[1].Value;
                string port = match.Groups[2].Value;

                if (int.TryParse(port, out int portValue))
                {
                    Console.WriteLine($"Host: {host}, Port: {portValue}");
                    return (host, portValue);
                }
            }

            throw new Exception($"The host or port is not valid: {mqttUrl}");

        }

    }
}
