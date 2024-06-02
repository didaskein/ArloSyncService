using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentSessionAnswer
    {
        public ArloAuthentSessionAnswer()
        {
            data = new Data();
        }

        public bool success { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public string userId { get; set; }
            public string email { get; set; }
            public string token { get; set; }
            public string paymentId { get; set; }
            public string accountStatus { get; set; }
            public string countryCode { get; set; }
            public bool tocUpdate { get; set; }
            public bool policyUpdate { get; set; }
            public bool validEmail { get; set; }
            public bool arlo { get; set; }
            public long dateCreated { get; set; }
            public bool mailProgramChecked { get; set; }
            public string mqttUrl { get; set; }
            public bool arloApp { get; set; }
            public bool supportsMultiLocation { get; set; }
            public bool canUserMigrate { get; set; }
        }
    }




}
