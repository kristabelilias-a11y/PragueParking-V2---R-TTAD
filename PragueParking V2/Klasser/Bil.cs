using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking_V2.Klasser
{
    public class Bil : Fordon
    {
        public Bil(string regNr) : base(regNr)
        {
            FordonsTyp = "BIL";
        }
    }
}
