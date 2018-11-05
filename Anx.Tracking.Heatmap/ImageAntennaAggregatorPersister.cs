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
        private const int pointSize = 10;

        private const int Longitudes = 360;
        private const int Latitudes = 180;
        private const int PixelsPerDegree = 24;

        private int counter = 0;
        private readonly Bitmap img;
        private readonly Graphics g;

        private readonly int width;
        private readonly int height;

        private readonly string folderPath;

        private ImageAntennaAggregatorPersister(string folderPath)
        {
            this.folderPath = folderPath;

            width = Longitudes * PixelsPerDegree;
            height = Latitudes * PixelsPerDegree;
            img = new Bitmap(width, height);
            g = Graphics.FromImage(img);

            var segment = 45;
            var pen = new Pen(Brushes.Red, 5);
            for (int i = 1; i < Longitudes / segment; i++)
            {
                g.DrawLine(pen, segment * i * PixelsPerDegree, 0, segment * i * PixelsPerDegree, height);
            }

            for (int i = 1; i < Latitudes / segment; i++)
            {
                g.DrawLine(pen, 0, segment * i * PixelsPerDegree, width, segment * i * PixelsPerDegree);
            }

            g.Flush();
        }

        public void Dispose()
        {

        }

        public Task Persist(ImageAntennaAggregator agg)
        {
            var points = agg.GetAllPoints();
            if(points.Count == 0)
                return Task.CompletedTask;

            for (int i = 0; i < points.Count; i++)
            {
                ApplyPoint(points[i]);
            }

            g.Flush();
            using (var fs = File.Open(Path.Combine(folderPath, $"biglayer.png"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                img.Save(fs, ImageFormat.Png);
            }

            return Task.CompletedTask;
        }

        private void ApplyPoint(Point p)
        {
            counter++;
            var x = Math.Min((int)((p.Lat + Latitudes / 2) * PixelsPerDegree), height - 1);
            var y = Math.Min((int)((p.Lon + Longitudes / 2) * PixelsPerDegree), width - 1);
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

        public static ImageAntennaAggregatorPersister Create(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            else
            {
                foreach (var file in Directory.EnumerateFiles(folderPath))
                {
                    File.Delete(file);
                }
            }

            return new ImageAntennaAggregatorPersister(folderPath);
        }
    }
}
