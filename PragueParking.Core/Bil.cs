using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class Bil : Fordon
    {
        public Bil() : base()
        {
            FordonsTyp = "BIL";
            Storlek = 4;
        }
        public Bil(string regNr) : base(regNr)
        {
            FordonsTyp = "BIL";
            Storlek = 4;
        }
    }
}
