using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public interface ISqlGeographyPersister
    {
        Task Persist(SqlGeographyAntennaAggregator agg);
    }
}
