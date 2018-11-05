using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class Tracking
    {
        public Tracking(string antenna, DateTime timestamp, Point point, int alt)
        {
            Antenna = antenna;
            Timestamp = timestamp;
            Point = point;
            Alt = alt;
        }

        public string Antenna { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Point Point { get; private set; }
        public int Alt { get; private set; }
    }
}
