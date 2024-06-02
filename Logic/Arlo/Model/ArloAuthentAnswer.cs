using ArloSyncService.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentAnswer
    {
        public ArloAuthentAnswer()
        {

            meta = new Meta();
            data = new Data();
        }

        public Meta meta { get; set; }
        public Data data { get; set; }

        public class Meta
        {
            public int code { get; set; }
            public string message { get; set; }

        }

        public class Data
        {
            public string _type { get; set; }
            public string token { get; set; }
            public string userId { get; set; }
            public int authenticated { get; set; }

            [JsonConverter(typeof(UnixSecondsDateTimeConverter))]
            public DateTime issued { get; set; }

            [JsonConverter(typeof(UnixSecondsDateTimeConverter))]
            public DateTime expiresIn { get; set; }

            public bool mfa { get; set; }
            public bool authCompleted { get; set; }
            public string type { get; set; }
            public string MFA_State { get; set; }
        }
    }



}
