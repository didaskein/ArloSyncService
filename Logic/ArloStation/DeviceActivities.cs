using ArloSyncService.Logic.Arlo.Model;
using ArloSyncService.Logic.ArloStation;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BF.Services.Logic
{
    public class DeviceActivities
    {
        //The Key is the DeviceId (deviceId not the UniqueId)  https://learn.microsoft.com/fr-fr/dotnet/standard/collections/thread-safe/blockingcollection-overview
        private readonly static ConcurrentQueue<DeviceEvent> devicesEvents = new ConcurrentQueue<DeviceEvent>();
        private readonly static ConcurrentDictionary<string, DeviceInfo> devicesInfos = new ConcurrentDictionary<string, DeviceInfo>();
        public bool isInitialized = false;

        public void Initialize(List<ArloDevicesAnswer.Datum> devices)
        {
            foreach (var device in devices)
            {
                var di = new DeviceInfo();
                di.xCloudId = device.xCloudId;
                di.DeviceId = device.deviceId;
                di.DeviceName = device.deviceName;
                di.DeviceType = device.deviceType;
                di.UniqueId = device.uniqueId;
                di.ParentId = device.parentId;
                di.UserId = device.userId;
                di.AllowedMqttTopics = device.allowedMqttTopics;

                devicesInfos.TryAdd(device.deviceId, di);
            }

            isInitialized = true;
        }

        public DeviceInfo GetDeviceInfo(string deviceId)
        {
            return devicesInfos.GetOrAdd(deviceId, (accId) => new DeviceInfo());
        }

        public void AddInfoToaDevice(string deviceId, DeviceInfo deviceInfo)
        {
            if (devicesInfos.ContainsKey(deviceId))
            {
                devicesInfos[deviceId] = deviceInfo;
            }
            else
            {
                devicesInfos.TryAdd(deviceId, deviceInfo);
            }
        }

        public void RemoveDevice(string deviceId)
        {
            devicesInfos.TryRemove(deviceId, out _);
        }

        public void UpdatePortIpStation(string deviceId, string privateIP, int port)
        {
            var device = GetDeviceInfo(deviceId);
            device.HostPrivateIP = privateIP;
            device.Port = port;
        }

        public void UpdateTokenStation(string deviceId, string token, DateTime tokenValidTo)
        {
            var device = GetDeviceInfo(deviceId);
            device.Token = token;
            device.TokenValidTo = tokenValidTo;
        }


        public List<string> GetDiscinctListofMqttChannels()
        {
            var mqttChannels = new List<string>();
            foreach (var device in devicesInfos)
            {
                foreach (var topic in device.Value.AllowedMqttTopics)
                {
                    if (!mqttChannels.Contains(topic))
                    {
                        mqttChannels.Add(topic);
                    }
                }
            }
            return mqttChannels;
        }


        public bool AllStationHaveIpAndPort()
        {
            int nbBaseStation = devicesInfos.Where(o => o.Value.DeviceType == "basestation").Count();

            int nbBaseStationWithPortAndPrivateIp = devicesInfos.Where(o => o.Value.DeviceType == "basestation" && o.Value.Port != 0 && !string.IsNullOrWhiteSpace(o.Value.HostPrivateIP)).Count();

            if (nbBaseStationWithPortAndPrivateIp == nbBaseStation) return true;
            else return false;
        }

        public List<DeviceInfo> GetStations()
        {
            var stations = devicesInfos.Values.Where(o => o.DeviceType == "basestation").ToList();
            return stations;
        }

        public void EnqueueDeviceEvent(DeviceEvent deviceEvent)
        {
            devicesEvents.Enqueue(deviceEvent);
        }

        public bool DequeueDeviceEvent(out DeviceEvent deviceEvent)
        {
            return devicesEvents.TryDequeue(out deviceEvent);
        }
    }
}
