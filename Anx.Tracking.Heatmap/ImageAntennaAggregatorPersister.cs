using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public class ImageAntennaAggregatorPersister : IAggregatorPersister<ImageAntennaAggregator>, IDisposable
    {
        [DebuggerDisplay("{LatStart}, {LonStart} ({counter})")]
        private class SectorHandler : IDisposable
        {
            private const int pointSize = 10;

            private int counter = 0;
            private readonly Bitmap img;
            private readonly Graphics g;
            public SectorHandler(int width, int height, int latStart, int lonStart)
            {
                Width = width;
                Height = height;
                LatStart = latStart;
                LonStart = lonStart;
                img = new Bitmap(width, height);
                g = Graphics.FromImage(img);
            }

            public int Width { get; private set; }
            public int Height { get; private set; }
            public int LatStart { get; private set; }
            public int LonStart { get; private set; }

            public void ApplyPoint(Point p)
            {
                counter++;
                var x = Math.Min((int)((p.Lat - LatStart) * PixelsPerDegree), Width - 1);
                var y = Math.Min((int)((p.Lon - LonStart) * PixelsPerDegree), Height - 1);
                var x1 = x - pointSize / 2;
                var y1 = y - pointSize / 2;
                g.FillEllipse(GetBrush(x1, y1), x1, y1, pointSize, pointSize);
            }

            private Brush GetBrush(int x, int y)
            {
                var gradientPath = new GraphicsPath();
                gradientPath.AddEllipse(x, y, pointSize, pointSize);
                var brush = new PathGradientBrush(gradientPath);
                brush.CenterColor = Color.FromArgb(100, Color.Black);
                brush.SurroundColors = new[] { Color.FromArgb(0, Color.Black) };
                return brush;
            }

            public void Dispose()
            {
                g.Dispose();
                img.Dispose();
            }

            public void Save(string folderPath)
            {
                if (counter == 0)
                    return;

                g.Flush();
                using (var fs = File.Open(Path.Combine(folderPath, $"{LatStart}_{LonStart}.png"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    img.Save(fs, ImageFormat.Png);
                }
            }
        }

        private const int Longitudes = 360;
        private const int Latitudes = 180;
        private const int PixelsPerDegree = 24;

        private readonly int SectorHeight;
        private readonly int SectorWidth;

        private readonly string folderPath;

        private readonly SectorHandler[,] sectors;
        private readonly int latSectors;
        private readonly int lonSectors;

        public ImageAntennaAggregatorPersister(string folderPath, int latSectors = 4, int lonSectors = 8)
        {
            this.folderPath = folderPath;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            else
            {
                foreach (var file in Directory.EnumerateFiles(folderPath))
                {
                    File.Delete(file);
                }
            }

            this.latSectors = latSectors;
            this.lonSectors = lonSectors;
            SectorHeight = Latitudes / latSectors;
            SectorWidth = Longitudes / lonSectors;
            sectors = new SectorHandler[latSectors, lonSectors];
            for (int x = 0; x < lonSectors; x++)
            {
                for (int y = 0; y < latSectors; y++)
                {
                    sectors[y, x] = new SectorHandler(
                        width: SectorWidth * PixelsPerDegree,
                        height: SectorHeight * PixelsPerDegree,
                        latStart: -90 + y * SectorHeight,
                        lonStart: -180 + x * SectorWidth
                    );
                }
            }
        }

        private IEnumerable<SectorHandler> GetAllHandlers()
        {
            for (int y = 0; y < sectors.GetLength(0); y++)
            {
                for (int x = 0; x < sectors.GetLength(1); x++)
                {
                    yield return sectors[y, x];
                }
            }
        }
        
        private SectorHandler GetSectorMap(Point p)
        {
            var x = (int)((p.Lon + 180) / SectorWidth);
            var y = (int)((p.Lat + 90) / SectorHeight);
            return sectors[y, x];
        }

        public void Dispose()
        {
            foreach (var item in GetAllHandlers())
            {
                item.Dispose();
            }
        }

        public Task Persist(ImageAntennaAggregator agg)
        {
            var points = agg.GetAllPoints();
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                GetSectorMap(p).ApplyPoint(p);
            }

            foreach (var item in GetAllHandlers())
            {
                item.Save(folderPath);
            }

            return Task.CompletedTask;
        }
    }
}
