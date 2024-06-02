using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloDevicesAnswer
    {
        public ArloDevicesAnswer()
        {
            data = new List<Datum>();
        }

        public List<Datum> data { get; set; }
        public bool success { get; set; }

        public class Datum
        {
            public string userId { get; set; }
            public string deviceId { get; set; }
            public string parentId { get; set; }
            public string uniqueId { get; set; }
            public string deviceType { get; set; }
            public string deviceName { get; set; }
            public long lastModified { get; set; }
            public string firmwareVersion { get; set; }
            public string xCloudId { get; set; }
            public string lastImageUploaded { get; set; }
            public string userRole { get; set; }
            public int displayOrder { get; set; }
            public string presignedLastImageUrl { get; set; }
            public string presignedSnapshotUrl { get; set; }
            public string presignedFullFrameSnapshotUrl { get; set; }
            public int mediaObjectCount { get; set; }
            public string state { get; set; }
            public string modelId { get; set; }
            public long dateCreated { get; set; }
            public string interfaceVersion { get; set; }
            public string interfaceSchemaVer { get; set; }
            public Owner owner { get; set; }
            public Properties properties { get; set; }
            public List<string> allowedMqttTopics { get; set; }
            public States states { get; set; }
            public Connectivity1 connectivity { get; set; }
            public bool certAvailable { get; set; }
            public int automationRevision { get; set; }
            public bool migrateActivityZone { get; set; }
        }

        public class Owner
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string ownerId { get; set; }
        }

        public class Properties
        {
            public string serialNumber { get; set; }
            public string swVersion { get; set; }
            public string modelId { get; set; }
            public string connectionState { get; set; }
            public string olsonTimeZone { get; set; }
            public int signalStrength { get; set; }
            public bool claimed { get; set; }
            public string timeZone { get; set; }
            public int interfaceVersion { get; set; }
            public List<Resource> resources { get; set; }
            public bool mqttSupport { get; set; }
            public string hwVersion { get; set; }
            public int apiVersion { get; set; }
            public List<Connectivity> connectivity { get; set; }
            public string activityState { get; set; }
            public string state { get; set; }
            public string timeSynchState { get; set; }
            public object updateAvailable { get; set; }
            public string updateCountryCode { get; set; }
            public Antiflicker antiFlicker { get; set; }
        }

        public class Antiflicker
        {
            public int mode { get; set; }
            public int autoDefault { get; set; }
        }

        public class Resource
        {
            public int duration { get; set; }
            public int volume { get; set; }
            public string sirenType { get; set; }
            public string pattern { get; set; }
            public int ID { get; set; }
            public string sirenState { get; set; }
            public string type { get; set; }
            public int sirenTimestamp { get; set; }
        }

        public class Connectivity
        {
            public bool connected { get; set; }
            public int signalStrength { get; set; }
            public int wifiRssi { get; set; }
            public string type { get; set; }
            public string ssid { get; set; }
            public string ipAddr { get; set; }
        }

        public class States
        {
            public string activeMode { get; set; }
            public int schemaVersion { get; set; }
            public string source { get; set; }
        }

        public class Connectivity1
        {
            public string type { get; set; }
            public bool connected { get; set; }
            public string mepStatus { get; set; }
        }

    }




}
