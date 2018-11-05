using System;
using System.Globalization;
using Microsoft.SqlServer.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anx.Tracking.Heatmap.Tests
{
    [TestClass]
    public class TrackingParserTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
        }

        [DataRow("1540252799,AC9AAA,N911CX,21000,225,83,33.4872,-117.062,0,7353,kvny_a01,1540252797,A2,21600,0",
            "kvny_a01", "2018-10-22 23:59:57", 33.4872, -117.062)]
        [TestMethod]
        public void Successful_tracking_parsing(string input, string id, string timestampText, double lat, double lon)
        {
            // arrange
            var point = new Point(lat, lon);
            var timestamp = DateTime.Parse(timestampText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            var parser = new TrackingParser();

            // act
            var result = parser.Parse(input);

            // assert
            Assert.IsNotNull(result, message: "Unexpected missing tracking");
            Assert.AreEqual(expected: id, actual: result.Antenna, message: "Unexpected value for Antenna");
            Assert.AreEqual(expected: timestamp, actual: result.Timestamp, message: "Unexpected value for Timestamp");
            Assert.AreEqual(expected: point, actual: result.Point, message: "Unexpected value for Point");
        }
    }
}
