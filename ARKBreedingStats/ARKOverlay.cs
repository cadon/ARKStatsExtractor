using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class ARKOverlay : Form
    {
        private Label[] labels = new Label[10];
        public Process ARKProcess;

        public ARKOverlay()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            labels[0] = lblLevel;
            labels[1] = lblHealth;
            labels[2] = lblStamina;
            labels[3] = lblOxygen;
            labels[4] = lblFood;
            labels[5] = lblWeight;
            labels[6] = lblMeleeDamage;
            labels[7] = lblMovementSpeed;

            this.Location = Point.Empty;// this.PointToScreen(ArkOCR.OCR.statPositions["NameAndLevel"]);
            this.Size = new Size(2000, 2000);
        }

        public void setValues(float[] wildValues, float[] tamedValues)
        {
            foreach( KeyValuePair<String,Point> kv in ArkOCR.OCR.statPositions )
            {
                if (kv.Key == "Torpor")
                    continue;

                int statIndex = -1;
                switch( kv.Key )
                {
                    case "NameAndLevel": statIndex = 0; break;
                    case "Health": statIndex = 1;  break;
                    case "Stamina": statIndex = 2; break;
                    case "Oxygen": statIndex = 3; break;
                    case "Food": statIndex = 4; break;
                    case "Weight": statIndex = 5; break;
                    case "Melee Damage": statIndex = 6; break;
                    case "Movement Speed": statIndex = 7; break;
                    default:
                        break;
                }

                if (statIndex == -1)
                    continue;

                labels[statIndex].Text = "[w" + wildValues[statIndex];
                if (tamedValues[statIndex] != 0)
                    labels[statIndex].Text += " + d" + tamedValues[statIndex];
                labels[statIndex].Text += "]";
                labels[statIndex].Location = this.PointToClient(ArkOCR.OCR.lastLetterositions[kv.Key]);
            }
            
        }

    }
}
