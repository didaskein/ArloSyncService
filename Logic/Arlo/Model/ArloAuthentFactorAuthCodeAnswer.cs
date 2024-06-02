using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentFactorAuthCodeAnswer
    {
        public ArloAuthentFactorAuthCodeAnswer()
        {
            meta = new Meta();
            data = new Data();
        }
        public Meta meta { get; set; }
        public Data data { get; set; }

        public class Meta
        {
            public int code { get; set; }
        }

        public class Data
        {
            public string _type { get; set; }
            public string factorAuthCode { get; set; }
            public string MFA_State { get; set; }
        }

    }



}
