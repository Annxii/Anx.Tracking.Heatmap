using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap.TrackingFilters
{
    public class TrackingFilterCollection : ITrackingFilter
    {
        private readonly ITrackingFilter[] trackingFilters;

        public TrackingFilterCollection(params ITrackingFilter[] trackingFilters)
        {
            this.trackingFilters = trackingFilters;
        }

        public bool Include(Tracking tracking) => trackingFilters.Length == 0 || trackingFilters.All(x => x.Include(tracking));
    }
}
