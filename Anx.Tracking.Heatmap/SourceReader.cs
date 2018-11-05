using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class SourceReader
    {
        private readonly string[] paths;

        public SourceReader(params string[] paths)
        {
            this.paths = paths;
        }

        public IObservable<string> GetSource()
        {
            return Observable.Create<string>(obs =>
            {
                try
                {
                    var currentPath = string.Empty;
                    var counter = 0;
                    try
                    {
                        for (int i = 0; i < paths.Length; i++)
                        {
                            currentPath = paths[i];
                            counter = 0;
                            using (var rdr = new StreamReader(currentPath))
                            {
                                while (!rdr.EndOfStream)
                                {
                                    counter++;
                                    obs.OnNext(rdr.ReadLine());
                                }
                            }
                        }
                        obs.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException($"Fail read at '{currentPath}' line {counter}", ex);
                    }
                }
                catch (Exception ex)
                {
                    obs.OnError(ex);
                }

                return Disposable.Empty;
            });
        }
    }
}
