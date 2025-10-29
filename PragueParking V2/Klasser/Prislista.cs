using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking_V2.Klasser
{
    public static class Prislista
    {
        public static int BeräknaPris(Fordon fordon, DateTime utTid)
        {
            int prisPerTimme = fordon is Bil ? 20 : 10;

            TimeSpan tid = utTid - fordon.Incheckningstid;
            if (tid.TotalMinutes <= 10)
                return 0; // första 10 minuter gratis

            double timmar = Math.Ceiling(tid.TotalHours);
            return (int)timmar * prisPerTimme;
        }
    }
}
