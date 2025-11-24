using ARKBreedingStats.utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ARKBreedingStats.SpeciesImages
{
    internal class ImageComposition
    {
        private ImageCompositionPart[] _parts;

        public ImageCompositionPart[] Parts
        {
            get => _parts;
            set
            {
                _parts = value;
                Hash = Hashes.CombineOrderedHashes(_parts?.Select(p => p.GetHashCode()));
            }
        }

        public int Hash;

        public bool CombineImages(string[] filePaths, string filePathResult)
        {
            if (filePaths == null) return false;

            // TODO assuming fixed 256 × 256 px for now
            const int size = 256;
            try
            {
                using (var bmpBg = new Bitmap(size, size, PixelFormat.Format32bppArgb))
                using (var bmpMask = new Bitmap(size, size, PixelFormat.Format24bppRgb))
                using (var gBg = Graphics.FromImage(bmpBg))
                using (var gM = Graphics.FromImage(bmpMask))
                {
                    SetGraphicProperties(gBg);
                    SetGraphicProperties(gM);
                    var maskExists = false;
                    var i = 0;
                    foreach (var p in _parts)
                    {
                        var filePath = filePaths[i++];
                        DrawPart(filePath, p, gBg);
                        maskExists |= DrawPart(CreatureImageFile.MaskFilePath(filePath), p, gM, true);
                        if (p.BorderWidth <= 0) continue;
                        DrawBorder(p, gBg);
                        DrawBorder(p, gM, true);
                    }
                    bmpBg.Save(filePathResult);
                    if (maskExists)
                        bmpMask.Save(CreatureImageFile.MaskFilePath(filePathResult));
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error when combining species image and saving to\n" + filePathResult);
            }
            return false;
        }

        private static void SetGraphicProperties(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private static bool DrawPart(string filePath, ImageCompositionPart part, Graphics g, bool isMask = false)
        {
            if (!File.Exists(filePath)) return false;
            using (var bmpPart = new Bitmap(filePath))
            {
                if (isMask)
                    bmpPart.MakeTransparent(Color.Black);

                var partRectangle = bmpPart.GetBounds();
                part.InitializeDestinationPath(g.VisibleClipBounds);
                var rDest = part.DestinationPath.GetBounds();

                var rectangleSource = ImageCompositionPart.ArrayToRectangleF(part.SourceRectangle, out var r)
                    ? r
                    : partRectangle;

                g.SetClip(part.DestinationPath);
                if (part.Clear || part.BackgroundColor.A != 0)
                    g.Clear(isMask ? Color.Transparent : part.BackgroundColor);

                if (!isMask && ImageCompositionPart.ArrayToRectangleF(part.ShadowRectangle, rDest, out var rectangleShadow))
                    CreatureColored.DrawShadowEllipse(g, rectangleShadow, part.ShadowColor, part.ShadowAngle, part.ShadowIntensity);

                g.DrawImage(bmpPart, rDest, rectangleSource, GraphicsUnit.Pixel);
                g.ResetClip();
            }
            return true;
        }

        private static void DrawBorder(ImageCompositionPart part, Graphics g, bool useBlack = false)
        {
            using (var pen = new Pen(useBlack ? Color.Black : part.BorderColor, part.BorderWidth))
                g.DrawPath(pen, part.DestinationPath);
        }
    }
}
