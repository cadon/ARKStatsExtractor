using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.ocr.Common
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; }
        public int[] Bits { get; }
        public bool Disposed { get; private set; }
        public int Height { get; }
        public int Width { get; }

        protected GCHandle BitsHandle { get; }

        public float GetMeaningfulPixelsCoefficient => (float)this.PixelsWithData / (this.Height * this.Width);

        internal int PixelsWithData { get; set; }

        public DirectBitmap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Bits = new int[width * height];
            this.BitsHandle = GCHandle.Alloc(this.Bits, GCHandleType.Pinned);
            this.Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, this.BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color color)
        {
            int index = x + (y * this.Width);
            int col = color.ToArgb();

            this.Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * this.Width);
            int col = this.Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        internal Bitmap ToBitmap()
        {
            return (Bitmap)this.Bitmap.Clone();
        }

        public void Dispose()
        {
            if (this.Disposed) return;
            this.Disposed = true;
            this.Bitmap.Dispose();
            this.BitsHandle.Free();
        }

        public Color GetDominantColor()
        {
            long[] total = { 0, 0, 0 };
            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    int index = i + j * this.Width;
                    var color = Color.FromArgb(this.Bits[index]);
                    total[0] += color.R;
                    total[1] += color.G;
                    total[2] += color.B;
                }
            }

            total[0] /= this.Width * this.Height;
            total[1] /= this.Width * this.Height;
            total[2] /= this.Width * this.Height;
            return Color.FromArgb((int)total[0], (int)total[1], (int)total[2]);
        }

        public override int GetHashCode()
        {
            return this.Bits.Aggregate(this.Bits.Length, (current, t) => unchecked(current * 31 + t));
        }
    }
}
