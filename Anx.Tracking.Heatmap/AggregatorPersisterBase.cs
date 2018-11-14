using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public abstract class AggregatorPersisterBase<T> : IAggregatorPersister<T>, IDisposable
        where T : AntennaAggregator
    {
        public virtual void Dispose()
        {
        }

        public Task Persist(T agg)
        {
            Console.WriteLine($"Saving {agg.Antenna} to {GetType().Name}...");
            return PersistCore(agg);
        }

        protected abstract Task PersistCore(T agg);
    }
}
