using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public struct BoundingBox
    {
        public readonly static BoundingBox Empty = new BoundingBox(new Point(-90, 180), new Point(90, -180));

        public readonly Point TopLeft;
        public readonly Point BottomRight;

        public BoundingBox(Point topLeft, Point bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        public double Width => Math.Abs(BottomRight.Lat - TopLeft.Lat);
        public double Height => Math.Abs(TopLeft.Lon - BottomRight.Lon);

        public BoundingBox ApplyPoint(Point p)
        {
            var maxLat = Math.Max(p.Lat, TopLeft.Lat);
            var minLat = Math.Min(p.Lat, BottomRight.Lat);

            var maxLon = Math.Max(p.Lon, BottomRight.Lon);
            var minLon = Math.Min(p.Lon, TopLeft.Lon);
            return new BoundingBox(new Point(maxLat, minLon), new Point(minLat, maxLon));
        }

        public Point GetRelativePosition(Point p)
        {
            var lat = p.Lat - BottomRight.Lat;
            var lon = p.Lon - TopLeft.Lon;
            return new Point(lat, lon);
        }
    }
}
