using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.ocr.Common
{
    public static class ImageUtils
    {
        public static DirectBitmap GetAdjustedDirectBitmapOfImage(Image img, float brightnessAdj)
        {
            var db = new DirectBitmap(img.Width, img.Height);

            using (var graphics = Graphics.FromImage(db.Bitmap))
            {
                graphics.DrawImage(img, Point.Empty);
            }

            for (var i = 0; i < img.Width; i++)
            {
                for (var j = 0; j < img.Height; j++)
                {
                    var currentPixel = db.GetPixel(i, j);
                    var brightness = currentPixel.GetBrightness();
                    if (brightness < 0.55f * brightnessAdj)
                    {
                        db.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        db.SetPixel(i, j, Color.Black);
                        db.PixelsWithData++;
                    }
                }
            }

            return db;
        }
    }
}
