using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public interface IAggregatorPersister<T>
        where T : AntennaAggregator
    {
        Task Persist(T agg);
    }
}
