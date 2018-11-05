using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{

    public abstract class AntennaAggregator
    {
        protected AntennaAggregator(string antenna)
        {
            Antenna = antenna;
        }

        public string Antenna { get; private set; }
        public int Count { get; private set; }
        public AntennaAggregator AddPoint(Point point)
        {
            Count++;
            AddPointCore(point);
            return this;
        }

        protected abstract void AddPointCore(Point point);
    }
}
