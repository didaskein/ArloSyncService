using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentValidateAnswer
    {
        public ArloAuthentValidateAnswer()
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
            public Data()
            {
                interactions = new Interactions();
            }

            public string _type { get; set; }
            public string _id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string country { get; set; }
            public string language { get; set; }
            public int acceptedPolicy { get; set; }
            public int currentPolicy { get; set; }
            public bool emailConfirmed { get; set; }
            public string email { get; set; }
            public bool mfa { get; set; }
            public Interactions interactions { get; set; }
            public string mfaSetup { get; set; }
            public string MFA_State { get; set; }
            public bool tokenValidated { get; set; }
        }

        public class Interactions
        {
            public int mfaDeniedTimestamp { get; set; }
            public bool mfaRemindersEnabled { get; set; }
            public int serverTime { get; set; }
        }
    }




}
