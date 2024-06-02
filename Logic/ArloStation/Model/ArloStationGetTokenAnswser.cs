using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation.Model
{
    public class ArloStationGetTokenAnswser
    {

        public ArloStationGetTokenAnswser()
        {
            data = new Data();
        }

        public Data data { get; set; }
        public bool success { get; set; }

        public class Data
        {
            public string ratlsToken { get; set; }
        }
    }



}
