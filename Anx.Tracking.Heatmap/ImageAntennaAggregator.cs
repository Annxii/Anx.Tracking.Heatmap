using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class ImageAntennaAggregator : AntennaAggregator
    {
        private readonly List<Point> points = new List<Point>(1000);

        public ImageAntennaAggregator(string antenna) : base(antenna) { }

        protected override void AddPointCore(Point point)
        {
            points.Add(point);
        }

        public IReadOnlyList<Point> GetAllPoints() => points;
    }
}
