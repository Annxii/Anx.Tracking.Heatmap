using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class SqlGeographyAntennaAggregator : AntennaAggregator
    {
        private readonly GeographyConvexHullAggregate aggregator;

        public SqlGeographyAntennaAggregator(string antenna) : base(antenna)
        {
            aggregator = new GeographyConvexHullAggregate();
            aggregator.Init();
        }

        protected override void AddPointCore(Point point)
        {
            aggregator.Accumulate(SqlGeography.Point(point.Lat, point.Lon, 4326));
        }

        public SqlGeography GetSqlGeography() => aggregator.Terminate();
    }
}
