using PragueParking.Core;

namespace Tester
{

namespace PragueParking_Tester
    {
        [TestClass]
        public class ParkeringshusTester // Säkerställa att en bil kan parkeras i ett nytt tomt parkeringshus
        {
            [TestMethod]
            public void FörsökParkera_BilPåLedigPlats_ReturnerarTrue()
            {
                // Arrange
                var parkeringshus = new Parkeringshus();
                var bil = new Bil("ABC123");

                // Act
                bool resultat = parkeringshus.FörsökParkera(bil, out string meddelande);

                // Assert
                Assert.IsTrue(resultat, "Bilen borde ha kunnat parkeras på en ledig plats.");
                Assert.IsTrue(meddelande.Contains("parkerad"), "Meddelandet ska indikera lyckad parkering.");
            }
        }

        [TestClass]
        public class ParkeringshusTester2 // Förhindra att ett fordon med samma regnr parkeras två gånger
        {
            public class DubbelparkeringTester
            {
                [TestMethod]
                public void FörsökParkera_SammaBilTvåGånger_ReturnerarFalse()
                {
                    // Arrange
                    var parkeringshus = new Parkeringshus();
                    var bil = new Bil("XYZ999");

                    // Act
                    parkeringshus.FörsökParkera(bil, out _); // Första gången går bra
                    bool resultat2 = parkeringshus.FörsökParkera(new Bil("XYZ999"), out string meddelande2); // Försök igen

                    // Assert
                    Assert.IsFalse(resultat2, "Samma bil ska inte kunna parkeras två gånger.");
                    Assert.IsTrue(meddelande2.Contains("finns redan"), "Meddelandet ska indikera att bilen redan finns.");
                }
            }
        }
    }
}