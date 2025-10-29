using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking_V2.Klasser
{
    public class MC : Fordon
    {
        public MC(string regNr) : base(regNr)
        {
            FordonsTyp = "MC";
            Storlek = 2;
        }
    }
}
