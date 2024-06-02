using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArloSyncService.Logic.ArloStation.Model.ArloStationCreateCertAnswser;

namespace ArloSyncService.Logic.ArloStation
{
    public class DeviceInfo
    {
        public DeviceInfo()
        {
            AllowedMqttTopics = new List<string>();
        }

        public string DeviceId { get; set; }
        public string UniqueId { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set; }

        public string xCloudId { get; set; }

        public string ParentId { get; set; }

        public string UserId { get; set; }


        public List<string> AllowedMqttTopics { get; set; }



        public int Port { get; set; }
        public string HostPrivateIP { get; set; }

        public string? Token { get; set; }
        public DateTime? TokenValidTo { get; set; }

        public string StationUrl => $"https://{HostPrivateIP}:{Port}/hmsls";
    }
}
