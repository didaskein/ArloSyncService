using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation.Model
{
    public class ArloStationOpenPortAnswser
    {
        public string from { get; set; }
        public string to { get; set; }
        public string transId { get; set; }
        public string action { get; set; }
        public string resource { get; set; }
        public Properties properties { get; set; }
        public bool success { get; set; }

        public class Properties
        {
            public string privateIP { get; set; }
            public string publicIP { get; set; }
            public int port { get; set; }
        }
    }

}
