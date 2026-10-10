using ARKBreedingStats.uiControls;

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
            listViewPlayer = new System.Windows.Forms.ListView();
            columnHeaderRank = new System.Windows.Forms.ColumnHeader();
            columnHeaderPlayer = new System.Windows.Forms.ColumnHeader();
            columnHeaderLevel = new System.Windows.Forms.ColumnHeader();
            columnHeaderTribe = new System.Windows.Forms.ColumnHeader();
            columnHeaderRel = new System.Windows.Forms.ColumnHeader();
            columnHeaderNotes = new System.Windows.Forms.ColumnHeader();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panelSettings = new System.Windows.Forms.Panel();
            panelPlayerSettings = new System.Windows.Forms.Panel();
            nudPlayerRank = new Nud();
            label9 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            textBoxPlayerTribe = new System.Windows.Forms.TextBox();
            textBoxPlayerName = new System.Windows.Forms.TextBox();
            textBoxPlayerNotes = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            nudPlayerLevel = new Nud();
            panelTribeSettings = new System.Windows.Forms.Panel();
            label7 = new System.Windows.Forms.Label();
            groupBox1 = new GroupBoxC();
            radioButtonFriendly = new System.Windows.Forms.RadioButton();
            radioButtonHostile = new System.Windows.Forms.RadioButton();
            radioButtonNeutral = new System.Windows.Forms.RadioButton();
            radioButtonAllied = new System.Windows.Forms.RadioButton();
            textBoxTribeNotes = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            textBoxTribeName = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            groupBox2 = new GroupBoxC();
            listViewTribes = new System.Windows.Forms.ListView();
            columnHeaderName = new System.Windows.Forms.ColumnHeader();
            columnHeaderRelation = new System.Windows.Forms.ColumnHeader();
            tableLayoutPanel1.SuspendLayout();
            panelSettings.SuspendLayout();
            panelPlayerSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPlayerRank).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPlayerLevel).BeginInit();
            panelTribeSettings.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // listViewPlayer
            // 
            listViewPlayer.AllowColumnReorder = true;
            listViewPlayer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderRank, columnHeaderPlayer, columnHeaderLevel, columnHeaderTribe, columnHeaderRel, columnHeaderNotes });
            listViewPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewPlayer.FullRowSelect = true;
            listViewPlayer.Location = new System.Drawing.Point(280, 3);
            listViewPlayer.Name = "listViewPlayer";
            tableLayoutPanel1.SetRowSpan(listViewPlayer, 2);
            listViewPlayer.Size = new System.Drawing.Size(462, 503);
            listViewPlayer.TabIndex = 1;
            listViewPlayer.UseCompatibleStateImageBehavior = false;
            listViewPlayer.View = System.Windows.Forms.View.Details;
            listViewPlayer.ColumnClick += listView_ColumnClick;
            listViewPlayer.SelectedIndexChanged += listViewPlayer_SelectedIndexChanged;
            listViewPlayer.Enter += listViewPlayer_Enter;
            listViewPlayer.KeyUp += listViewPlayer_KeyUp;
            // 
            // columnHeaderRank
            // 
            columnHeaderRank.Text = "Rank";
            columnHeaderRank.Width = 38;
            // 
            // columnHeaderPlayer
            // 
            columnHeaderPlayer.Text = "Name";
            columnHeaderPlayer.Width = 85;
            // 
            // columnHeaderLevel
            // 
            columnHeaderLevel.Text = "Level";
            columnHeaderLevel.Width = 38;
            // 
            // columnHeaderTribe
            // 
            columnHeaderTribe.Text = "Tribe";
            columnHeaderTribe.Width = 77;
            // 
            // columnHeaderRel
            // 
            columnHeaderRel.Text = "Relation";
            columnHeaderRel.Width = 51;
            // 
            // columnHeaderNotes
            // 
            columnHeaderNotes.Text = "Notes";
            columnHeaderNotes.Width = 198;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 277F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panelSettings, 0, 0);
            tableLayoutPanel1.Controls.Add(listViewPlayer, 1, 0);
            tableLayoutPanel1.Controls.Add(groupBox2, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 214F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(745, 509);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panelSettings
            // 
            panelSettings.Controls.Add(panelPlayerSettings);
            panelSettings.Controls.Add(panelTribeSettings);
            panelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            panelSettings.Location = new System.Drawing.Point(3, 3);
            panelSettings.Name = "panelSettings";
            panelSettings.Size = new System.Drawing.Size(271, 208);
            panelSettings.TabIndex = 11;
            // 
            // panelPlayerSettings
            // 
            panelPlayerSettings.Controls.Add(nudPlayerRank);
            panelPlayerSettings.Controls.Add(label9);
            panelPlayerSettings.Controls.Add(label8);
            panelPlayerSettings.Controls.Add(label1);
            panelPlayerSettings.Controls.Add(textBoxPlayerTribe);
            panelPlayerSettings.Controls.Add(textBoxPlayerName);
            panelPlayerSettings.Controls.Add(textBoxPlayerNotes);
            panelPlayerSettings.Controls.Add(label2);
            panelPlayerSettings.Controls.Add(label4);
            panelPlayerSettings.Controls.Add(label3);
            panelPlayerSettings.Controls.Add(nudPlayerLevel);
            panelPlayerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            panelPlayerSettings.Enabled = false;
            panelPlayerSettings.Location = new System.Drawing.Point(0, 0);
            panelPlayerSettings.Name = "panelPlayerSettings";
            panelPlayerSettings.Size = new System.Drawing.Size(271, 208);
            panelPlayerSettings.TabIndex = 8;
            // 
            // nudPlayerRank
            // 
            nudPlayerRank.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudPlayerRank.Location = new System.Drawing.Point(196, 56);
            nudPlayerRank.Name = "nudPlayerRank";
            nudPlayerRank.Size = new System.Drawing.Size(63, 23);
            nudPlayerRank.TabIndex = 14;
            nudPlayerRank.ValueChanged += nudPlayerRank_ValueChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(157, 58);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(33, 15);
            label9.TabIndex = 13;
            label9.Text = "Rank";
            // 
            // label8
            // 
            label8.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label8.Location = new System.Drawing.Point(3, 5);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(265, 19);
            label8.TabIndex = 11;
            label8.Text = "Player";
            label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 34);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(69, 15);
            label1.TabIndex = 0;
            label1.Text = "Playername";
            // 
            // textBoxPlayerTribe
            // 
            textBoxPlayerTribe.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            textBoxPlayerTribe.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            textBoxPlayerTribe.Location = new System.Drawing.Point(78, 82);
            textBoxPlayerTribe.Name = "textBoxPlayerTribe";
            textBoxPlayerTribe.Size = new System.Drawing.Size(181, 23);
            textBoxPlayerTribe.TabIndex = 5;
            textBoxPlayerTribe.TextChanged += textBoxPlayerTribe_TextChanged;
            // 
            // textBoxPlayerName
            // 
            textBoxPlayerName.Location = new System.Drawing.Point(78, 30);
            textBoxPlayerName.Name = "textBoxPlayerName";
            textBoxPlayerName.Size = new System.Drawing.Size(181, 23);
            textBoxPlayerName.TabIndex = 1;
            textBoxPlayerName.TextChanged += textBoxPlayerName_TextChanged;
            // 
            // textBoxPlayerNotes
            // 
            textBoxPlayerNotes.Location = new System.Drawing.Point(47, 108);
            textBoxPlayerNotes.Multiline = true;
            textBoxPlayerNotes.Name = "textBoxPlayerNotes";
            textBoxPlayerNotes.Size = new System.Drawing.Size(212, 91);
            textBoxPlayerNotes.TabIndex = 7;
            textBoxPlayerNotes.TextChanged += textBoxPlayerNotes_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 86);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(33, 15);
            label2.TabIndex = 4;
            label2.Text = "Tribe";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 112);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(38, 15);
            label4.TabIndex = 6;
            label4.Text = "Notes";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 59);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(34, 15);
            label3.TabIndex = 2;
            label3.Text = "Level";
            // 
            // nudPlayerLevel
            // 
            nudPlayerLevel.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudPlayerLevel.Location = new System.Drawing.Point(78, 56);
            nudPlayerLevel.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudPlayerLevel.Name = "nudPlayerLevel";
            nudPlayerLevel.Size = new System.Drawing.Size(73, 23);
            nudPlayerLevel.TabIndex = 3;
            nudPlayerLevel.ValueChanged += nudPlayerLevel_ValueChanged;
            // 
            // panelTribeSettings
            // 
            panelTribeSettings.Controls.Add(label7);
            panelTribeSettings.Controls.Add(groupBox1);
            panelTribeSettings.Controls.Add(textBoxTribeNotes);
            panelTribeSettings.Controls.Add(label6);
            panelTribeSettings.Controls.Add(textBoxTribeName);
            panelTribeSettings.Controls.Add(label5);
            panelTribeSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            panelTribeSettings.Enabled = false;
            panelTribeSettings.Location = new System.Drawing.Point(0, 0);
            panelTribeSettings.Name = "panelTribeSettings";
            panelTribeSettings.Size = new System.Drawing.Size(271, 208);
            panelTribeSettings.TabIndex = 9;
            // 
            // label7
            // 
            label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label7.Location = new System.Drawing.Point(3, 5);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(265, 19);
            label7.TabIndex = 10;
            label7.Text = "Tribe";
            label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioButtonFriendly);
            groupBox1.Controls.Add(radioButtonHostile);
            groupBox1.Controls.Add(radioButtonNeutral);
            groupBox1.Controls.Add(radioButtonAllied);
            groupBox1.Location = new System.Drawing.Point(66, 53);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(151, 66);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Relation";
            // 
            // radioButtonFriendly
            // 
            radioButtonFriendly.AutoSize = true;
            radioButtonFriendly.Location = new System.Drawing.Point(6, 42);
            radioButtonFriendly.Name = "radioButtonFriendly";
            radioButtonFriendly.Size = new System.Drawing.Size(67, 19);
            radioButtonFriendly.TabIndex = 1;
            radioButtonFriendly.TabStop = true;
            radioButtonFriendly.Text = "Friendly";
            radioButtonFriendly.UseVisualStyleBackColor = true;
            radioButtonFriendly.CheckedChanged += radioButtonFriendly_CheckedChanged;
            // 
            // radioButtonHostile
            // 
            radioButtonHostile.AutoSize = true;
            radioButtonHostile.Location = new System.Drawing.Point(86, 42);
            radioButtonHostile.Name = "radioButtonHostile";
            radioButtonHostile.Size = new System.Drawing.Size(62, 19);
            radioButtonHostile.TabIndex = 3;
            radioButtonHostile.TabStop = true;
            radioButtonHostile.Text = "Hostile";
            radioButtonHostile.UseVisualStyleBackColor = true;
            radioButtonHostile.CheckedChanged += radioButtonHostile_CheckedChanged;
            // 
            // radioButtonNeutral
            // 
            radioButtonNeutral.AutoSize = true;
            radioButtonNeutral.Location = new System.Drawing.Point(86, 19);
            radioButtonNeutral.Name = "radioButtonNeutral";
            radioButtonNeutral.Size = new System.Drawing.Size(64, 19);
            radioButtonNeutral.TabIndex = 2;
            radioButtonNeutral.TabStop = true;
            radioButtonNeutral.Text = "Neutral";
            radioButtonNeutral.UseVisualStyleBackColor = true;
            radioButtonNeutral.CheckedChanged += radioButtonNeutral_CheckedChanged;
            // 
            // radioButtonAllied
            // 
            radioButtonAllied.AutoSize = true;
            radioButtonAllied.Location = new System.Drawing.Point(6, 19);
            radioButtonAllied.Name = "radioButtonAllied";
            radioButtonAllied.Size = new System.Drawing.Size(55, 19);
            radioButtonAllied.TabIndex = 0;
            radioButtonAllied.TabStop = true;
            radioButtonAllied.Text = "Allied";
            radioButtonAllied.UseVisualStyleBackColor = true;
            radioButtonAllied.CheckedChanged += radioButtonAllied_CheckedChanged;
            // 
            // textBoxTribeNotes
            // 
            textBoxTribeNotes.Location = new System.Drawing.Point(66, 125);
            textBoxTribeNotes.Multiline = true;
            textBoxTribeNotes.Name = "textBoxTribeNotes";
            textBoxTribeNotes.Size = new System.Drawing.Size(188, 72);
            textBoxTribeNotes.TabIndex = 9;
            textBoxTribeNotes.TextChanged += textBoxTribeNotes_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(3, 128);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(38, 15);
            label6.TabIndex = 8;
            label6.Text = "Notes";
            // 
            // textBoxTribeName
            // 
            textBoxTribeName.Location = new System.Drawing.Point(66, 27);
            textBoxTribeName.Name = "textBoxTribeName";
            textBoxTribeName.Size = new System.Drawing.Size(188, 23);
            textBoxTribeName.TabIndex = 6;
            textBoxTribeName.TextChanged += textBoxTribeName_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(3, 30);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(63, 15);
            label5.TabIndex = 5;
            label5.Text = "Tribename";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(listViewTribes);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(3, 217);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(271, 289);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Tribes";
            // 
            // listViewTribes
            // 
            listViewTribes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderName, columnHeaderRelation });
            listViewTribes.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewTribes.FullRowSelect = true;
            listViewTribes.Location = new System.Drawing.Point(3, 19);
            listViewTribes.MultiSelect = false;
            listViewTribes.Name = "listViewTribes";
            listViewTribes.Size = new System.Drawing.Size(265, 267);
            listViewTribes.TabIndex = 0;
            listViewTribes.UseCompatibleStateImageBehavior = false;
            listViewTribes.View = System.Windows.Forms.View.Details;
            listViewTribes.ColumnClick += listView_ColumnClick;
            listViewTribes.SelectedIndexChanged += listViewTribes_SelectedIndexChanged;
            listViewTribes.Enter += listViewTribes_Enter;
            listViewTribes.KeyUp += listViewTribes_KeyUp;
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Name";
            columnHeaderName.Width = 199;
            // 
            // columnHeaderRelation
            // 
            columnHeaderRelation.Text = "Relation";
            columnHeaderRelation.Width = 54;
            // 
            // TribesControl
            // 
            Controls.Add(tableLayoutPanel1);
            Name = "TribesControl";
            Size = new System.Drawing.Size(745, 509);
            tableLayoutPanel1.ResumeLayout(false);
            panelSettings.ResumeLayout(false);
            panelPlayerSettings.ResumeLayout(false);
            panelPlayerSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPlayerRank).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPlayerLevel).EndInit();
            panelTribeSettings.ResumeLayout(false);
            panelTribeSettings.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewPlayer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ColumnHeader columnHeaderPlayer;
        private System.Windows.Forms.ColumnHeader columnHeaderLevel;
        private System.Windows.Forms.ColumnHeader columnHeaderTribe;
        private System.Windows.Forms.ColumnHeader columnHeaderRel;
        private System.Windows.Forms.ColumnHeader columnHeaderNotes;
        private GroupBoxC groupBox2;
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
        private uiControls.Nud nudPlayerLevel;
        private System.Windows.Forms.Panel panelTribeSettings;
        private System.Windows.Forms.Label label7;
        private GroupBoxC groupBox1;
        private System.Windows.Forms.RadioButton radioButtonFriendly;
        private System.Windows.Forms.RadioButton radioButtonHostile;
        private System.Windows.Forms.RadioButton radioButtonNeutral;
        private System.Windows.Forms.RadioButton radioButtonAllied;
        private System.Windows.Forms.TextBox textBoxTribeNotes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxTribeName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private uiControls.Nud nudPlayerRank;
        private System.Windows.Forms.ColumnHeader columnHeaderRank;
    }
}
