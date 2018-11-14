using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class JsonArrayAggregator : AntennaAggregator
    {
        private readonly StringBuilder b = new StringBuilder(1000); 

        public JsonArrayAggregator(string antenna) : base(antenna)
        {
            b.AppendLine("var datapoints = [");
        }

        protected override void AddPointCore(Point point)
        {
            b.AppendLine($"\t{{\"lat\": {point.Lat.ToString(CultureInfo.InvariantCulture)}, \"lon\": {point.Lon.ToString(CultureInfo.InvariantCulture)} }}");
        }

        public string GetJsonArray()
        {
            b.AppendLine("];");
            return b.ToString();
        }
    }
}
