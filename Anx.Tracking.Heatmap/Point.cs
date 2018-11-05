using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public struct Point : IEquatable<Point>
    {
        public readonly double Lat;
        public readonly double Lon;

        public Point(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public bool Equals(Point other) => Lat == other.Lat && Lon == other.Lon;
    }
}
