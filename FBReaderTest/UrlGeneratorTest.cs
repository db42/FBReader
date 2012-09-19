using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using FBReader;
using FBReader.Services;

namespace FBReaderTest
{
    [TestClass]
    public class UrlGeneratorTest
    {
        [TestMethod]
        public void ProfileUrlValid()
        {
            UrlGenerator urlGenerator = new UrlGenerator();
            string username = "dushyant";
            string access_token = "access_token=1234";
            string resultProfileUrl = "https://graph.facebook.com/dushyant?access_token=1234&fields=id,name,relationship_status,friends.fields(relationship_status,gender,name)";
            Assert.AreEqual(urlGenerator.constructProfileUrl(username, access_token) , resultProfileUrl);
        }

    }
}
