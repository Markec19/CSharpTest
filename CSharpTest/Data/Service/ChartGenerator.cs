
using CSharpTest.Models;
using SkiaSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSharpTest.Data.Service
{
    public class ChartGenerator
    {
        public static byte[] GeneratePieChart(IEnumerable<Employee> employees)
        {
            const int width = 800;
            const int height = 600;
            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            var total = employees.Sum(e => e.TimeWorked);
            var rect = new SKRect(100, 100, 500, 500);

            var random = new Random();
            var colors = employees.Select(_ => new SKColor(
                (byte)random.Next(50, 256),
                (byte)random.Next(50, 256),
                (byte)random.Next(50, 256)
            )).ToArray();

            float startAngle = 0;
            int colorIndex = 0;

            foreach (var e in employees)
            {
                float sweepAngle = (float)(e.TimeWorked / total) * 360f;

                using var segmentPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = colors[colorIndex % colors.Length]
                };

                canvas.DrawArc(rect, startAngle, sweepAngle, true, segmentPaint);

                // Izračun centra segmenta
                float midAngle = startAngle + sweepAngle / 2;
                double radians = Math.PI * midAngle / 180;
                float radius = 150f;
                float x = 300 + radius * (float)Math.Cos(radians);
                float y = 300 + radius * (float)Math.Sin(radians);

                float percent = (float)(e.TimeWorked / total) * 100f;
                string label = $"{percent:F1}%";

                using var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 16,
                    IsAntialias = true
                };

                // Centriraj tekst
                var bounds = new SKRect();
                textPaint.MeasureText(label, ref bounds);
                float centeredX = x - bounds.MidX;
                float centeredY = y - bounds.MidY;

                canvas.DrawText(label, centeredX, centeredY, textPaint);

                startAngle += sweepAngle;
                colorIndex++;
            }

            float legendX = 550;
            float legendY = 120;
            float spacing = 30;
            colorIndex = 0;

            foreach (var e in employees)
            {
                var legendColor = colors[colorIndex % colors.Length];

                using var legendPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = legendColor
                };

                canvas.DrawRect(legendX, legendY + colorIndex * spacing, 20, 20, legendPaint);

                using var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 18,
                    IsAntialias = true
                };

                canvas.DrawText(e.EmployeeName, legendX + 30, legendY + 16 + colorIndex * spacing, textPaint);

                colorIndex++;
            }

            using var image = surface.Snapshot();
            using var dataPng = image.Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream();
            dataPng.SaveTo(ms);
            return ms.ToArray();
        }

    }
}



