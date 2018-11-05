using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class TrackingParser
    {
        private readonly static DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly static Regex regex = new Regex(@"^[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,(?<lat>[+-]?\d+(\.\d+)?),(?<lon>[+-]?\d+(\.\d+)?),[^,]*,[^,]*,(?<id>[^,]+),(?<time>\d+)");

        private readonly ParserDebugSink debug;

        public TrackingParser(ParserDebugSink debug = null)
        {
            this.debug = debug;
        }


        public Tracking Parse(string input)
        {
            var m = regex.Match(input);
            if (!m.Success)
                return null;

            var id = m.Groups["id"].Value;
            var time = FromEpoch(m.Groups["time"].Value);
            var lat = double.Parse(m.Groups["lat"].Value, CultureInfo.InvariantCulture);
            var lon = double.Parse(m.Groups["lon"].Value, CultureInfo.InvariantCulture);
            var tracking = new Tracking(id, time, new Point(lat, lon), 0);

            if (debug?.Condition(tracking) ?? false)
                debug.Log(input, tracking);

            return tracking;
        }

        private DateTime FromEpoch(string value) => epochStart.AddSeconds(int.Parse(value));
    }
}
