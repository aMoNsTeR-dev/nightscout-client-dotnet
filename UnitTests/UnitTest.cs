namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        private static INightscoutRestApiClient _ns;


        [ClassInitialize]
        public static void InitOnce(TestContext context)
        {
            string nsUrl = context.Properties["nightscoutUrl"].ToString();
            string nsToken = context.Properties["tokenInUrl"].ToString();

            _ns = new NightscoutRestApiV1Client(nsUrl, NightscoutClientDotNet.Models.Enums.ApiKeyType.TokenInUrl, nsToken);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            // TODO Cleanup
        }


        [TestMethod]
        public async Task GetEntries()
        {
            IEnumerable<Entry> entries = await _ns.GetEntriesAsync("", 1);
            Assert.AreEqual(1, entries.Count());
        }
    }
}