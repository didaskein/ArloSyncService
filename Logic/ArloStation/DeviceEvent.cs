using ArloSyncService.Logic.ArloStation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation
{
    public class DeviceEvent
    {
        public DeviceEvent()
        {
        }

        public string DeviceId { get; set; }
        public string UniqueId { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set; }

        public string xCloudId { get; set; }

        public string ParentId { get; set; }
        public string Message { get; set; }

        public ArloStationNewRecordAnswser Record { get; set; }

        //Add other Fields
    }
}
