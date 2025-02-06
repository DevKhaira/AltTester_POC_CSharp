using OpenQA.Selenium.Appium.Android;

using AltBy = AltTester.AltTesterUnitySDK.Driver.By;
using allinHole_tests_csharp.tests;

namespace allinhole_gameplay_csharp.tests
{
    public class Gameplay
    {
        public AltDriver altDriver;

        [OneTimeSetUp]
        public void SetupAltTester()
        {
            // Start AltTester Driver
            altDriver = new AltDriver(port: 13000);
            Console.WriteLine("AltDriver started on port 13000");

            // Look for the specific UI element using AltTester
            try
            {
                var element = altDriver.FindObject(AltBy.PATH, "/UI/Game/Game/Top/Safe Area 2/iOS Safe Area/Content/Goals/ItemsSlots/Slot");
                Console.WriteLine("Element found: " + element.name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Element not found: " + ex.Message);
            }
        }

        [TearDown]
        public void KeepAltTesterAlive()
        {
            Console.WriteLine("Keeping AltTester session active");
        }

        [OneTimeTearDown]
        public void DisposeAltTester()
        {
            Console.WriteLine("Ending AltTester session");
            
            if (altDriver != null)
            {
                altDriver.Stop();
            }
        }
    }

    public class SimpleTouchTest : BaseTest
    {
        [Test]
        public void TestTouchScreenMovement()
        {   
            caseId = 2978;
            // /Level[1]/New Game Object/Apricot(Clone)(Clone)[2]/Apricot(Clone)/_UMS_LODs_/Level01/000_static_Apricot(Clone)_combined_static
            var path =  "/UI/Game/Game/Top/Safe Area 2/iOS Safe Area/Content/Timer/Text_01";
            //UI/Game/Game/Top/Safe Area 2/iOS Safe Area/Content/Timer/Text_01
            var initialPosition = altDriver.FindObject(AltBy.PATH, path).GetText();
            // altDriver.FindObject(By.NAME, "Consent").Tap();
            Console.WriteLine("Initial position: " + initialPosition);
            altDriver.Swipe(new AltVector2(200, 300), new AltVector2(300, 400), 500, false);
            altDriver.Swipe(new AltVector2(300, 400), new AltVector2(200, 300), 500, false);
            Console.WriteLine("Swiped from (200, 300) to (300, 400)");
            Thread.Sleep(2000); // Allow time for movement
            
            var newPosition = altDriver.FindObject(AltBy.PATH, path).GetText();
            Console.WriteLine("New position: " + newPosition);

            // Thread.Sleep(10000); // Allow time for movement
            // Assert.Fail();
            Assert.That(initialPosition, Is.Not.EqualTo(newPosition), "The position should change after a screen tap.");
            Console.WriteLine("Assertion complete");
        }
    }

}
