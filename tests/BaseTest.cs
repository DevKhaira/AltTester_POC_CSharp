namespace allinHole_tests_csharp.tests
{
    public class BaseTest
    {
        public AltDriver altDriver;

        [SetUp]
        public void Setup()
        {
            // Initialize AltDriver (AltTester connection)
            altDriver = new AltDriver(port: 13000);
            Console.WriteLine("AltDriver started on port 13000");
        }

        [TearDown]
        public void Cleanup()
        {
            if (altDriver != null)
            {
                altDriver.Stop();
                Console.WriteLine("AltDriver stopped");
            }
        }
    }
}
