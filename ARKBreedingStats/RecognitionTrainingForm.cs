using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.ocr.Common;

namespace ARKBreedingStats
{
    public partial class RecognitionTrainingForm : Form
    {
        private char selectedChar;

        public RecognitionTrainingForm(RecognizedCharData charData, Image originalImg)
        {
            InitializeComponent();

            this.DrawPattern(charData.Pattern);

            Image adjustedOriginalPicture = this.DrawFoundCharOnPicture(originalImg, charData);

            this.pictureBox2.Image = adjustedOriginalPicture;
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

            this.pictureBox1.Image = b;
        }

        public char Prompt()
        {
            this.ShowDialog();

            return this.selectedChar;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;

            if (tb.Text.Length == 0)
            {
                this.selectedChar = '\0';
                return;
            }

            if (tb.Text.Length > 1)
            {
                tb.Text = tb.Text.Substring(tb.Text.Length - 1);
            }

            this.selectedChar = tb.Text[0];
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1.PerformClick();
            }
        }
    }
}
