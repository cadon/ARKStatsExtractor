using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public partial class RecognitionTrainingForm : Form
    {
        private string _selectedText;

        public RecognitionTrainingForm(RecognizedCharData charData, Image originalImg)
        {
            InitializeComponent();

            DrawPattern(charData.Pattern);

            Image adjustedOriginalPicture = DrawFoundCharOnPicture(originalImg, charData);

            pictureBox2.Image = adjustedOriginalPicture;
            _selectedText = string.Empty;
        }

        private Image DrawFoundCharOnPicture(Image img, RecognizedCharData charData)
        {
            using (var db = new DirectBitmap(img.Width, img.Height))
            {

                using (var graphics = Graphics.FromImage(db.Bitmap))
                {
                    graphics.DrawImage(img, Point.Empty);
                }

                using (var graphics = Graphics.FromImage(db.Bitmap))
                {
                    graphics.DrawImage(img, Point.Empty);
                }

                var xMax = charData.Coords.X + charData.Pattern.GetLength(0);
                var yMax = charData.Coords.Y + charData.Pattern.GetLength(1);

                var x = 0;
                for (var i = charData.Coords.X; i < xMax; i++, x++)
                {
                    var y = 0;
                    for (var j = charData.Coords.Y; j < yMax; j++, y++)
                    {
                        if (charData.Pattern[x, y])
                        {
                            db.SetPixel(i, j, Color.Red);
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
    }
}
