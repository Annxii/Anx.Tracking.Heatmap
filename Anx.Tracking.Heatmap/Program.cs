using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SqlServer.Types;
using Anx.Tracking.Heatmap.TrackingFilters;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Anx.Tracking.Heatmap
{
    class Program
    {
        static void Main(string[] args)
        {
            //ImageTest();
            //return;
            Execute().GetAwaiter().GetResult();
            Console.Write("Done! Press any key to close...");
            Console.ReadKey();
        }

        static void ImageTest()
        {
            var size = 400;
            using (var img = new Bitmap(size, size))
            using (var g = Graphics.FromImage(img))
            {
                var path = new GraphicsPath();
                path.AddEllipse(0, 0, size, size);
                var brush = new PathGradientBrush(path);
                brush.CenterColor = Color.Green;
                brush.SurroundColors = new[] { Color.FromArgb(0, Color.Green) };
                img.MakeTransparent();
                g.FillEllipse(brush, 0, 0, size, size);

                g.Flush();
                using (var fs = File.Open("test.png", FileMode.Create, FileAccess.ReadWrite))
                {
                    img.Save(fs, ImageFormat.Png);
                }
            }
        }

        static async Task Execute()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            using (var disp = new System.Reactive.Disposables.CompositeDisposable())
            using (var log = new StreamWriter(File.Open("log.txt", FileMode.Create, FileAccess.ReadWrite)))
            {
                var filter = new TrackingFilterCollection();// new TrackingFilterCollection(new AntennaTrackingFilter("ekbi_a01"));
                var source = new SourceReader(@"C:\Users\torst\Downloads\trackings\2018-10-23.csv");
                var parser = new TrackingParser(new ParserDebugSink(t => t.Point.Lat == 0, s => log.WriteLine(s)));
                using (var persister = new SqlGeographyAntennaAggregatorPersister("output.txt"))
                using (var imgPersister = new ImageAntennaAggregatorPersister("images"))
                {
                    var trackingCount = 0;
                    var antennaCount = 0;
                    var antennaGroups = source.GetSource()
                        .Where(x => x.Length < 200)
                        .Select(x => parser.Parse(x))
                        .Where(x => x != null && filter.Include(x))
                        .Do(x =>
                        {
                            if ((++trackingCount % 100000) == 0)
                                Console.WriteLine(trackingCount);
                        })
                        .GroupBy(x => x.Antenna)
                        .Publish();

                    antennaGroups.Do(x => antennaCount++)
                        .SubscribeDisposable(disp);

                    antennaGroups
                        .SelectMany(x => x.Aggregate(
                            new SqlGeographyAntennaAggregator(x.Key)
                            , (s, t) => s.AddPoint(t.Point) as SqlGeographyAntennaAggregator)
                        ).SelectMany(x => Observable.FromAsync(() => persister.Persist(x)))
                        .SubscribeDisposable(disp);

                    antennaGroups
                        .SelectMany(x => x.Aggregate(
                            new ImageAntennaAggregator(x.Key)
                            , (s, t) => s.AddPoint(t.Point) as ImageAntennaAggregator)
                        ).SelectMany(x => Observable.FromAsync(() => imgPersister.Persist(x)))
                        .SubscribeDisposable(disp);

                    antennaGroups.ConnectDisposable(disp);

                    await antennaGroups.LastOrDefaultAsync();

                    Console.WriteLine($"Total count: {trackingCount} - Antenna count: {antennaCount}");
                }
            }
        }
    }
}
