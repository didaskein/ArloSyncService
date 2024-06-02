using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation.Model
{
    public class ArloStationNewRecordAnswser
    {
        public ArloStationNewRecordAnswser()
        {
            properties = new Properties();
            properties.media = new List<Medium>();
        }

        public string from { get; set; }
        public string transId { get; set; }
        public string action { get; set; }
        public string resource { get; set; }
        public Properties properties { get; set; }
        public class Properties
        {
            public bool localRecordingActive { get; set; }
            public List<Medium> media { get; set; }
        }

        public class Medium
        {
            public string deviceId { get; set; }
            public string label { get; set; }
            public string uuid { get; set; }
            public string type { get; set; }
            public string fs { get; set; }
            public string status { get; set; }
            public bool recycle { get; set; }
            public long size { get; set; }
            public long free { get; set; }
        }
    }



}
