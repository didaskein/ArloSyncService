using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentFactorRequest
    {
        public ArloAuthentFactorRequest(string factorId)
        {
            this.factorId = factorId;
        }

        public string factorId { get; set; }

    }

}
