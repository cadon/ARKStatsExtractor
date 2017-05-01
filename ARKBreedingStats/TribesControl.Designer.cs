namespace ARKBreedingStats
{
    partial class TribesControl
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
            this.listViewPlayer = new System.Windows.Forms.ListView();
            this.columnHeaderPlayer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLvl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTribe = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderNotes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.panelPlayerSettings = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPlayerTribe = new System.Windows.Forms.TextBox();
            this.textBoxPlayerName = new System.Windows.Forms.TextBox();
            this.textBoxPlayerNotes = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownLevel = new System.Windows.Forms.NumericUpDown();
            this.panelTribeSettings = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonFriendly = new System.Windows.Forms.RadioButton();
            this.radioButtonHostile = new System.Windows.Forms.RadioButton();
            this.radioButtonNeutral = new System.Windows.Forms.RadioButton();
            this.radioButtonAllied = new System.Windows.Forms.RadioButton();
            this.textBoxTribeNotes = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxTribeName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listViewTribes = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRelation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.panelPlayerSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
            this.panelTribeSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewPlayer
            // 
            this.listViewPlayer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderPlayer,
            this.columnHeaderLvl,
            this.columnHeaderTribe,
            this.columnHeaderRel,
            this.columnHeaderNotes});
            this.listViewPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPlayer.FullRowSelect = true;
            this.listViewPlayer.HideSelection = false;
            this.listViewPlayer.Location = new System.Drawing.Point(280, 3);
            this.listViewPlayer.Name = "listViewPlayer";
            this.tableLayoutPanel1.SetRowSpan(this.listViewPlayer, 2);
            this.listViewPlayer.Size = new System.Drawing.Size(462, 503);
            this.listViewPlayer.TabIndex = 1;
            this.listViewPlayer.UseCompatibleStateImageBehavior = false;
            this.listViewPlayer.View = System.Windows.Forms.View.Details;
            this.listViewPlayer.SelectedIndexChanged += new System.EventHandler(this.listViewPlayer_SelectedIndexChanged);
            this.listViewPlayer.Enter += new System.EventHandler(this.listViewPlayer_Enter);
            this.listViewPlayer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewPlayer_KeyUp);
            // 
            // columnHeaderPlayer
            // 
            this.columnHeaderPlayer.Text = "Player";
            this.columnHeaderPlayer.Width = 85;
            // 
            // columnHeaderLvl
            // 
            this.columnHeaderLvl.Text = "Level";
            this.columnHeaderLvl.Width = 38;
            // 
            // columnHeaderTribe
            // 
            this.columnHeaderTribe.Text = "Tribe";
            this.columnHeaderTribe.Width = 77;
            // 
            // columnHeaderRel
            // 
            this.columnHeaderRel.Text = "Relation";
            this.columnHeaderRel.Width = 51;
            // 
            // columnHeaderNotes
            // 
            this.columnHeaderNotes.Text = "Notes";
            this.columnHeaderNotes.Width = 198;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 277F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelSettings, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listViewPlayer, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 214F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(745, 509);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelSettings
            // 
            this.panelSettings.Controls.Add(this.panelPlayerSettings);
            this.panelSettings.Controls.Add(this.panelTribeSettings);
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSettings.Location = new System.Drawing.Point(3, 3);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(271, 208);
            this.panelSettings.TabIndex = 11;
            // 
            // panelPlayerSettings
            // 
            this.panelPlayerSettings.Controls.Add(this.label8);
            this.panelPlayerSettings.Controls.Add(this.label1);
            this.panelPlayerSettings.Controls.Add(this.textBoxPlayerTribe);
            this.panelPlayerSettings.Controls.Add(this.textBoxPlayerName);
            this.panelPlayerSettings.Controls.Add(this.textBoxPlayerNotes);
            this.panelPlayerSettings.Controls.Add(this.label2);
            this.panelPlayerSettings.Controls.Add(this.label4);
            this.panelPlayerSettings.Controls.Add(this.label3);
            this.panelPlayerSettings.Controls.Add(this.numericUpDownLevel);
            this.panelPlayerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPlayerSettings.Enabled = false;
            this.panelPlayerSettings.Location = new System.Drawing.Point(0, 0);
            this.panelPlayerSettings.Name = "panelPlayerSettings";
            this.panelPlayerSettings.Size = new System.Drawing.Size(271, 208);
            this.panelPlayerSettings.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(265, 19);
            this.label8.TabIndex = 11;
            this.label8.Text = "Player";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Playername";
            // 
            // textBoxPlayerTribe
            // 
            this.textBoxPlayerTribe.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxPlayerTribe.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxPlayerTribe.Location = new System.Drawing.Point(66, 82);
            this.textBoxPlayerTribe.Name = "textBoxPlayerTribe";
            this.textBoxPlayerTribe.Size = new System.Drawing.Size(193, 20);
            this.textBoxPlayerTribe.TabIndex = 5;
            this.textBoxPlayerTribe.TextChanged += new System.EventHandler(this.textBoxPlayerTribe_TextChanged);
            // 
            // textBoxPlayerName
            // 
            this.textBoxPlayerName.Location = new System.Drawing.Point(66, 30);
            this.textBoxPlayerName.Name = "textBoxPlayerName";
            this.textBoxPlayerName.Size = new System.Drawing.Size(193, 20);
            this.textBoxPlayerName.TabIndex = 1;
            this.textBoxPlayerName.TextChanged += new System.EventHandler(this.textBoxPlayerName_TextChanged);
            // 
            // textBoxPlayerNotes
            // 
            this.textBoxPlayerNotes.Location = new System.Drawing.Point(66, 108);
            this.textBoxPlayerNotes.Multiline = true;
            this.textBoxPlayerNotes.Name = "textBoxPlayerNotes";
            this.textBoxPlayerNotes.Size = new System.Drawing.Size(193, 91);
            this.textBoxPlayerNotes.TabIndex = 7;
            this.textBoxPlayerNotes.TextChanged += new System.EventHandler(this.textBoxPlayerNotes_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tribe";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Notes";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Level";
            // 
            // numericUpDownLevel
            // 
            this.numericUpDownLevel.Location = new System.Drawing.Point(66, 56);
            this.numericUpDownLevel.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownLevel.Name = "numericUpDownLevel";
            this.numericUpDownLevel.Size = new System.Drawing.Size(77, 20);
            this.numericUpDownLevel.TabIndex = 3;
            this.numericUpDownLevel.ValueChanged += new System.EventHandler(this.numericUpDownLevel_ValueChanged);
            // 
            // panelTribeSettings
            // 
            this.panelTribeSettings.Controls.Add(this.label7);
            this.panelTribeSettings.Controls.Add(this.groupBox1);
            this.panelTribeSettings.Controls.Add(this.textBoxTribeNotes);
            this.panelTribeSettings.Controls.Add(this.label6);
            this.panelTribeSettings.Controls.Add(this.textBoxTribeName);
            this.panelTribeSettings.Controls.Add(this.label5);
            this.panelTribeSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTribeSettings.Enabled = false;
            this.panelTribeSettings.Location = new System.Drawing.Point(0, 0);
            this.panelTribeSettings.Name = "panelTribeSettings";
            this.panelTribeSettings.Size = new System.Drawing.Size(271, 208);
            this.panelTribeSettings.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(265, 19);
            this.label7.TabIndex = 10;
            this.label7.Text = "Tribe";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonFriendly);
            this.groupBox1.Controls.Add(this.radioButtonHostile);
            this.groupBox1.Controls.Add(this.radioButtonNeutral);
            this.groupBox1.Controls.Add(this.radioButtonAllied);
            this.groupBox1.Location = new System.Drawing.Point(66, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(151, 66);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Relation";
            // 
            // radioButtonFriendly
            // 
            this.radioButtonFriendly.AutoSize = true;
            this.radioButtonFriendly.Location = new System.Drawing.Point(6, 42);
            this.radioButtonFriendly.Name = "radioButtonFriendly";
            this.radioButtonFriendly.Size = new System.Drawing.Size(61, 17);
            this.radioButtonFriendly.TabIndex = 1;
            this.radioButtonFriendly.TabStop = true;
            this.radioButtonFriendly.Text = "Friendly";
            this.radioButtonFriendly.UseVisualStyleBackColor = true;
            this.radioButtonFriendly.CheckedChanged += new System.EventHandler(this.radioButtonFriendly_CheckedChanged);
            // 
            // radioButtonHostile
            // 
            this.radioButtonHostile.AutoSize = true;
            this.radioButtonHostile.Location = new System.Drawing.Point(86, 42);
            this.radioButtonHostile.Name = "radioButtonHostile";
            this.radioButtonHostile.Size = new System.Drawing.Size(57, 17);
            this.radioButtonHostile.TabIndex = 3;
            this.radioButtonHostile.TabStop = true;
            this.radioButtonHostile.Text = "Hostile";
            this.radioButtonHostile.UseVisualStyleBackColor = true;
            this.radioButtonHostile.CheckedChanged += new System.EventHandler(this.radioButtonHostile_CheckedChanged);
            // 
            // radioButtonNeutral
            // 
            this.radioButtonNeutral.AutoSize = true;
            this.radioButtonNeutral.Location = new System.Drawing.Point(86, 19);
            this.radioButtonNeutral.Name = "radioButtonNeutral";
            this.radioButtonNeutral.Size = new System.Drawing.Size(59, 17);
            this.radioButtonNeutral.TabIndex = 2;
            this.radioButtonNeutral.TabStop = true;
            this.radioButtonNeutral.Text = "Neutral";
            this.radioButtonNeutral.UseVisualStyleBackColor = true;
            this.radioButtonNeutral.CheckedChanged += new System.EventHandler(this.radioButtonNeutral_CheckedChanged);
            // 
            // radioButtonAllied
            // 
            this.radioButtonAllied.AutoSize = true;
            this.radioButtonAllied.Location = new System.Drawing.Point(6, 19);
            this.radioButtonAllied.Name = "radioButtonAllied";
            this.radioButtonAllied.Size = new System.Drawing.Size(50, 17);
            this.radioButtonAllied.TabIndex = 0;
            this.radioButtonAllied.TabStop = true;
            this.radioButtonAllied.Text = "Allied";
            this.radioButtonAllied.UseVisualStyleBackColor = true;
            this.radioButtonAllied.CheckedChanged += new System.EventHandler(this.radioButtonAllied_CheckedChanged);
            // 
            // textBoxTribeNotes
            // 
            this.textBoxTribeNotes.Location = new System.Drawing.Point(66, 125);
            this.textBoxTribeNotes.Multiline = true;
            this.textBoxTribeNotes.Name = "textBoxTribeNotes";
            this.textBoxTribeNotes.Size = new System.Drawing.Size(188, 72);
            this.textBoxTribeNotes.TabIndex = 9;
            this.textBoxTribeNotes.TextChanged += new System.EventHandler(this.textBoxTribeNotes_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Notes";
            // 
            // textBoxTribeName
            // 
            this.textBoxTribeName.Location = new System.Drawing.Point(66, 27);
            this.textBoxTribeName.Name = "textBoxTribeName";
            this.textBoxTribeName.Size = new System.Drawing.Size(188, 20);
            this.textBoxTribeName.TabIndex = 6;
            this.textBoxTribeName.TextChanged += new System.EventHandler(this.textBoxTribeName_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Tribename";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listViewTribes);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 217);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(271, 289);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tribes";
            // 
            // listViewTribes
            // 
            this.listViewTribes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderRelation});
            this.listViewTribes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTribes.FullRowSelect = true;
            this.listViewTribes.HideSelection = false;
            this.listViewTribes.Location = new System.Drawing.Point(3, 16);
            this.listViewTribes.MultiSelect = false;
            this.listViewTribes.Name = "listViewTribes";
            this.listViewTribes.Size = new System.Drawing.Size(265, 270);
            this.listViewTribes.TabIndex = 0;
            this.listViewTribes.UseCompatibleStateImageBehavior = false;
            this.listViewTribes.View = System.Windows.Forms.View.Details;
            this.listViewTribes.SelectedIndexChanged += new System.EventHandler(this.listViewTribes_SelectedIndexChanged);
            this.listViewTribes.Enter += new System.EventHandler(this.listViewTribes_Enter);
            this.listViewTribes.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewTribes_KeyUp);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 199;
            // 
            // columnHeaderRelation
            // 
            this.columnHeaderRelation.Text = "Relation";
            this.columnHeaderRelation.Width = 54;
            // 
            // TribesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TribesControl";
            this.Size = new System.Drawing.Size(745, 509);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelSettings.ResumeLayout(false);
            this.panelPlayerSettings.ResumeLayout(false);
            this.panelPlayerSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).EndInit();
            this.panelTribeSettings.ResumeLayout(false);
            this.panelTribeSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewPlayer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ColumnHeader columnHeaderPlayer;
        private System.Windows.Forms.ColumnHeader columnHeaderLvl;
        private System.Windows.Forms.ColumnHeader columnHeaderTribe;
        private System.Windows.Forms.ColumnHeader columnHeaderRel;
        private System.Windows.Forms.ColumnHeader columnHeaderNotes;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listViewTribes;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderRelation;
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.Panel panelPlayerSettings;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPlayerTribe;
        private System.Windows.Forms.TextBox textBoxPlayerName;
        private System.Windows.Forms.TextBox textBoxPlayerNotes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownLevel;
        private System.Windows.Forms.Panel panelTribeSettings;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonFriendly;
        private System.Windows.Forms.RadioButton radioButtonHostile;
        private System.Windows.Forms.RadioButton radioButtonNeutral;
        private System.Windows.Forms.RadioButton radioButtonAllied;
        private System.Windows.Forms.TextBox textBoxTribeNotes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxTribeName;
        private System.Windows.Forms.Label label5;
    }
}
