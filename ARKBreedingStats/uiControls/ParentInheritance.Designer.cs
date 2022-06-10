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
            this.components = new System.ComponentModel.Container();
            this.ControlMother = new PedigreeCreature();
            this.GbParents = new System.Windows.Forms.GroupBox();
            this.ControlFather = new PedigreeCreature();
            this.ControlOffspring = new PedigreeCreature();
            this.pedigreeCreatureHeaders = new PedigreeCreature();
            this.GbParents.SuspendLayout();
            this.SuspendLayout();
            // 
            // ControlMother
            // 
            this.ControlMother.Creature = null;
            this.ControlMother.Location = new System.Drawing.Point(6, 60);
            this.ControlMother.Name = "ControlMother";
            this.ControlMother.OnlyLevels = false;
            this.ControlMother.Size = new System.Drawing.Size(325, 35);
            this.ControlMother.TabIndex = 1;
            this.ControlMother.TotalLevelUnknown = false;
            // 
            // GbParents
            // 
            this.GbParents.Controls.Add(this.pedigreeCreatureHeaders);
            this.GbParents.Controls.Add(this.ControlFather);
            this.GbParents.Controls.Add(this.ControlOffspring);
            this.GbParents.Controls.Add(this.ControlMother);
            this.GbParents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GbParents.Location = new System.Drawing.Point(0, 0);
            this.GbParents.Name = "GbParents";
            this.GbParents.Size = new System.Drawing.Size(337, 182);
            this.GbParents.TabIndex = 2;
            this.GbParents.TabStop = false;
            this.GbParents.Text = "Parents";
            // 
            // ControlFather
            // 
            this.ControlFather.Creature = null;
            this.ControlFather.Location = new System.Drawing.Point(6, 142);
            this.ControlFather.Name = "ControlFather";
            this.ControlFather.OnlyLevels = false;
            this.ControlFather.Size = new System.Drawing.Size(325, 35);
            this.ControlFather.TabIndex = 3;
            this.ControlFather.TotalLevelUnknown = false;
            // 
            // ControlOffspring
            // 
            this.ControlOffspring.Creature = null;
            this.ControlOffspring.Location = new System.Drawing.Point(6, 101);
            this.ControlOffspring.Name = "ControlOffspring";
            this.ControlOffspring.OnlyLevels = false;
            this.ControlOffspring.Size = new System.Drawing.Size(325, 35);
            this.ControlOffspring.TabIndex = 2;
            this.ControlOffspring.TotalLevelUnknown = false;
            // 
            // pedigreeCreatureHeaders
            // 
            this.pedigreeCreatureHeaders.Creature = null;
            this.pedigreeCreatureHeaders.Location = new System.Drawing.Point(6, 19);
            this.pedigreeCreatureHeaders.Name = "pedigreeCreatureHeaders";
            this.pedigreeCreatureHeaders.OnlyLevels = false;
            this.pedigreeCreatureHeaders.Size = new System.Drawing.Size(325, 35);
            this.pedigreeCreatureHeaders.TabIndex = 4;
            this.pedigreeCreatureHeaders.TotalLevelUnknown = false;
            // 
            // ParentInheritance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GbParents);
            this.Name = "ParentInheritance";
            this.Size = new System.Drawing.Size(337, 182);
            this.GbParents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PedigreeCreature ControlMother;
        private System.Windows.Forms.GroupBox GbParents;
        private PedigreeCreature ControlFather;
        private PedigreeCreature ControlOffspring;
        private PedigreeCreature pedigreeCreatureHeaders;
    }
}
