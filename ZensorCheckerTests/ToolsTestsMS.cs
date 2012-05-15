using System.Linq;
using System.Net;
using apophis.ZensorChecker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heijden.DNS;

namespace ZensorCheckerTests
{
    [TestClass]
    public class ToolsTestsMS
    {


        [TestMethod]
        public void CountryISPTest()
        {
            CountryISP isp = new CountryISP();

            Assert.IsNotNull(isp.ExternalIP);
            Assert.AreEqual("CABLECOM GMBH", isp.Isp);
            Assert.AreEqual("SWITZERLAND", isp.Country);
            Assert.AreEqual("ZURICH", isp.Region);
            Assert.AreEqual("ZURICH", isp.City);
            Assert.AreEqual("UTC +01:00", isp.Timezone);
            Assert.AreEqual("DSL", isp.Networkspeed);
        }

        [TestMethod]
        public void DNSHelperTest()
        {
            Assert.AreEqual(IPAddress.Parse("208.67.222.222"), DNSHelper.OpenDNS1);
            Assert.AreEqual(IPAddress.Parse("208.67.220.220"), DNSHelper.OpenDNS2);

            var dnsServers = DNSHelper.GetLocalDNS();
            Assert.IsTrue(dnsServers.Any());

            foreach (var dnsServer in dnsServers)
            {
                //Request request = new Request();
                //request.AddQuestion(new Question("google.com", DnsType.ANAME, DnsClass.IN));
                //Response response = Resolver.Lookup(request, dnsServer);

                //var address = ((ANameRecord)response.Answers[0].Record).IPAddress;

                Assert.IsNotNull(address);
            }
        }
    }
}
