using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap.TrackingFilters
{
    public class AntennaTrackingFilter : ITrackingFilter
    {
        private readonly HashSet<string> antennas;

        public AntennaTrackingFilter(params string [] antennas)
        {
            this.antennas = new HashSet<string>(antennas);
        }

        public bool Include(Tracking tracking) => antennas.Contains(tracking.Antenna);
    }
}
