using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public partial class RecognitionTrainingForm : Form
    {
        private string _selectedText;

        public RecognitionTrainingForm(RecognizedCharData charData, bool[,] image)
        {
            InitializeComponent();

            DrawPattern(charData.Pattern);

            Image adjustedOriginalPicture = DrawFoundCharOnPicture(image, charData);

            pictureBox2.Image = adjustedOriginalPicture;
            _selectedText = string.Empty;
        }

        private Image DrawFoundCharOnPicture(bool[,] image, RecognizedCharData charData)
        {
            var w = image.GetLength(0);
            var h = image.GetLength(1);

            var charWidth = charData.Pattern.GetLength(0);
            var charYMax = charData.Coords.Y + charData.Pattern.GetLength(1);

            using (var db = new DirectBitmap(w, h))
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (image[x, y])
                            db.Bits[y * w + x] = -1; // white
                        else
                            db.Bits[y * w + x] = -16777216; // black
                    }

                    if (y >= charData.Coords.Y && y < charYMax)
                    {
                        var charY = y - charData.Coords.Y;
                        for (int x = 0; x < charWidth; x++)
                        {
                            if (charData.Pattern[x, charY])
                            {
                                db.Bits[y * w + x + charData.Coords.X] = -65536; // red
                            }
                        }
                    }
                }

                return db.ToBitmap();
            }
        }

        private void DrawPattern(bool[,] curPattern)
        {
            var rows = curPattern.GetLength(0);
            var columns = curPattern.GetLength(1);
            var mult = 1;
            if (rows < 30 && columns < 30)
            {
                mult = 5;
            }
            Bitmap b = new Bitmap(rows * mult, columns * mult);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    for (var mI = i * mult; mI < (i + 1) * mult; mI++)
                    {
                        for (var mJ = j * mult; mJ < (j + 1) * mult; mJ++)
                        {
                            b.SetPixel(mI, mJ, curPattern[i, j] ? Color.Black : Color.White);
                        }
                    }
                }
            }

            pictureBox1.Image = b;
        }

        public string Prompt()
        {
            ShowDialog();

            if (_selectedText == null)
                throw new OperationCanceledException("User canceled ocr");

            return _selectedText;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _selectedText = textBox1.Text;
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void BtAbort_Click(object sender, EventArgs e)
        {
            _selectedText = null;
            Close();
        }

        private void BtFemaleSign_Click(object sender, EventArgs e)
        {
            textBox1.Text += "♀";
        }

        private void BtMale_Click(object sender, EventArgs e)
        {
            textBox1.Text += "♂";
        }
    }
}
