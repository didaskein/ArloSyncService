using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation.Model
{
    public class ArloStationCreateCertAnswser
    {

        public ArloStationCreateCertAnswser()
        {
            data = new Data();
        }

        public Data data { get; set; }
        public bool success { get; set; }

        public class Data
        {
            public Data()
            {
                certsData = new List<Certsdata>();
            }
            public List<Certsdata> certsData { get; set; }
            public string icaCert { get; set; }
        }

        public class Certsdata
        {
            public string uniqueId { get; set; }
            public string peerCert { get; set; }
            public string deviceCert { get; set; }
            public string certId { get; set; }
        }

    }



}
