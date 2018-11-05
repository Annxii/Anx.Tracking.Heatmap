using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class ParserDebugSink
    {
        private readonly Predicate<Tracking> predicate;
        private readonly Action<string> logger;

        public ParserDebugSink(Predicate<Tracking> predicate, Action<string> logger)
        {
            this.predicate = predicate;
            this.logger = logger;
        }

        public bool Condition(Tracking tracking) => predicate.Invoke(tracking);
        public void Log(string input, Tracking tracking) => logger.Invoke(input);
    }
}
