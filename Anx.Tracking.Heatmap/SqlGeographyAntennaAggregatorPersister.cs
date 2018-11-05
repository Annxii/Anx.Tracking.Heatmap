using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Types;

namespace Anx.Tracking.Heatmap
{
    public class SqlGeographyAntennaAggregatorPersister : IAggregatorPersister<SqlGeographyAntennaAggregator>, IDisposable
    {
        private readonly string outputPath;
        private readonly StreamWriter writer;
        private int counter = 0;

        public SqlGeographyAntennaAggregatorPersister(string outputPath)
        {
            this.outputPath = outputPath;
            writer = new StreamWriter(File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite));
        }

        public void Dispose()
        {
            writer.Dispose();
        }

        public async Task Persist(SqlGeographyAntennaAggregator agg)
        {
            if (await TrySave(agg.Antenna, agg.Count, agg.GetSqlGeography(), counter > 0))
                counter++;

            writer.Flush();
        }

        private async Task<bool> TrySave(string antenna, int count, SqlGeography geography, bool doUnion)
        {
            var txt = new string(geography.STAsText().Value);
            if (txt == "FULLGLOBE")
            {
                Console.WriteLine($"Skipping '{antenna}' because of 'FULLGLOBE'");
                return false;
            }

            if (doUnion)
                await writer.WriteLineAsync("UNION ALL");

            await writer.WriteLineAsync($"SELECT '{antenna}' AS Antenna, {count} AS [Count], geography::STPolyFromText('{txt}', 4326) AS [Geo]");
            return true;
        }
    }
}
