using ARKBreedingStats.Pedigree;

namespace ARKBreedingStats.uiControls
{
    partial class ParentInheritance
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ControlMother = new PedigreeCreature();
            GbParents = new System.Windows.Forms.GroupBox();
            pedigreeCreatureHeaders = new PedigreeCreature();
            ControlFather = new PedigreeCreature();
            ControlOffspring = new PedigreeCreature();
            GbParents.SuspendLayout();
            SuspendLayout();
            // 
            // ControlMother
            // 
            ControlMother.Location = new System.Drawing.Point(7, 61);
            ControlMother.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            ControlMother.Name = "ControlMother";
            ControlMother.Size = new System.Drawing.Size(379, 55);
            ControlMother.TabIndex = 1;
            // 
            // GbParents
            // 
            GbParents.BackColor = System.Drawing.Color.Transparent;
            GbParents.Controls.Add(pedigreeCreatureHeaders);
            GbParents.Controls.Add(ControlFather);
            GbParents.Controls.Add(ControlOffspring);
            GbParents.Controls.Add(ControlMother);
            GbParents.Dock = System.Windows.Forms.DockStyle.Fill;
            GbParents.Location = new System.Drawing.Point(0, 0);
            GbParents.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GbParents.Name = "GbParents";
            GbParents.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GbParents.Size = new System.Drawing.Size(393, 254);
            GbParents.TabIndex = 2;
            GbParents.TabStop = false;
            GbParents.Text = "Parents";
            // 
            // pedigreeCreatureHeaders
            // 
            pedigreeCreatureHeaders.Location = new System.Drawing.Point(7, 17);
            pedigreeCreatureHeaders.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            pedigreeCreatureHeaders.Name = "pedigreeCreatureHeaders";
            pedigreeCreatureHeaders.Size = new System.Drawing.Size(379, 40);
            pedigreeCreatureHeaders.TabIndex = 4;
            // 
            // ControlFather
            // 
            ControlFather.Location = new System.Drawing.Point(7, 192);
            ControlFather.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            ControlFather.Name = "ControlFather";
            ControlFather.Size = new System.Drawing.Size(379, 55);
            ControlFather.TabIndex = 3;
            // 
            // ControlOffspring
            // 
            ControlOffspring.Location = new System.Drawing.Point(7, 126);
            ControlOffspring.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            ControlOffspring.Name = "ControlOffspring";
            ControlOffspring.Size = new System.Drawing.Size(379, 55);
            ControlOffspring.TabIndex = 2;
            // 
            // ParentInheritance
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(GbParents);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ParentInheritance";
            Size = new System.Drawing.Size(393, 254);
            GbParents.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private PedigreeCreature ControlMother;
        private System.Windows.Forms.GroupBox GbParents;
        private PedigreeCreature ControlFather;
        private PedigreeCreature ControlOffspring;
        private PedigreeCreature pedigreeCreatureHeaders;
    }
}
