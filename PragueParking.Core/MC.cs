using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class MC : Fordon
    {
        public MC() : base()
        {
            FordonsTyp = "MC";
            Storlek = 2;
        }
        public MC(string regNr) : base(regNr)
        {
            FordonsTyp = "MC";
            Storlek = 2;
        }
    }
}
