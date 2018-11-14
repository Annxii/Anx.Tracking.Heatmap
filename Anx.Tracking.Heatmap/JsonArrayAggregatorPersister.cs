using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    class JsonArrayAggregatorPersister : AggregatorPersisterBase<JsonArrayAggregator>
    {
        private readonly string folderPath;

        public JsonArrayAggregatorPersister(string folderPath)
        {
            this.folderPath = folderPath;
        }

        protected override async Task PersistCore(JsonArrayAggregator agg)
        {
            var json = agg.GetJsonArray();
            using (var fs = File.Open(Path.Combine(folderPath, $"{agg.Antenna}.js"), FileMode.Create, FileAccess.ReadWrite))
            using (var writer = new StreamWriter(fs))
            {
                await writer.WriteAsync(json);
            }
        }

        public static JsonArrayAggregatorPersister Create(string folderPath)
        {
            var di = new DirectoryInfo(folderPath);
            if (!di.Exists)
                di.Create();

            return new JsonArrayAggregatorPersister(folderPath);
        }
    }
}
