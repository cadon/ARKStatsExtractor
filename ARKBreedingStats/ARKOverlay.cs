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
            int count = 1;
            foreach( KeyValuePair<String,Point> kv in ArkOCR.OCR.statPositions )
            {
                if (kv.Key == "Torpor")
                    continue;

                Label theLabel = null;
                switch( kv.Key )
                {
                    case "NameAndLevel": theLabel = lblLevel; break;
                    case "Health": theLabel = lblHealth;  break;
                    case "Stamina": theLabel = lblStamina; break;
                    case "Oxygen": theLabel = lblOxygen; break;
                    case "Food": theLabel = lblFood; break;
                    case "Weight": theLabel = lblWeight; break;
                    case "MeleeDamage": theLabel = lblMeleeDamage; break;
                    case "MovementSpeed": theLabel = lblMovementSpeed; break;
                    default:
                        break;
                }

                if (theLabel == null)
                    continue;

                theLabel.Text = "[w" + wildValues[count];
                if (tamedValues[count] != 0)
                    theLabel.Text += " + d" + tamedValues[count];
                theLabel.Text += "]";
                theLabel.Location = this.PointToClient(kv.Value);
            }
            
        }

    }
}
