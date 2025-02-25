namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
public class DummyTest
{
    [TestMethod]
    [Description("This is an always passing test to make sure at least 1 test succeed")]
    public void FilterBypassTest() => Thread.Sleep(100);
}
