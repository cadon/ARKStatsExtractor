using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class ModValuesManager : Form
    {
        private Mod selectedMod;
        private CreatureCollection cc;

        public ModValuesManager()
        {
            InitializeComponent();
        }

        public CreatureCollection creatureCollection
        {
            set
            {
                cc = value;
                UpdateModListBox();
            }
        }

        private void BtLoadModFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Additional values-file (*.json)|*.json",
                InitialDirectory = Path.Combine(FileService.GetJsonPath(), "mods"),
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filename = dlg.FileName;
                // copy to json folder if loaded from somewhere else
                string modsFolder = Path.Combine(FileService.GetJsonPath(), "mods");
                if (!filename.StartsWith(modsFolder))
                {
                    try
                    {
                        string destination = Path.Combine(modsFolder, Path.GetFileName(filename));
                        File.Copy(filename, destination);
                        filename = destination;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Trying to copy the file to the application's json folder failed.\n" +
                                "The program won't be able to load it at its next start.\n\n" +
                                "Error message:\n\n" + ex.Message, "Copy file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (Values.TryLoadValuesFile(filename, setModFileName: true, out Values modValues))
                {
                    if (cc.ModList.Contains(modValues.mod))
                    {
                        MessageBox.Show("The mod\n" + modValues.mod.title + "\nis already loaded.", "Already loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        cc.ModList.Add(modValues.mod);
                        selectedMod = modValues.mod;
                        UpdateModListBox();
                    }
                }
            }
        }

        private void BtRemoveModFile_Click(object sender, EventArgs e)
        {
            if (selectedMod == null || cc?.ModList == null) return;

            if (cc.ModList.Remove(selectedMod))
                UpdateModListBox();
        }

        private void BtMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectedMod(-1);
        }

        private void BtMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectedMod(1);
        }

        private void MoveSelectedMod(int moveBy)
        {
            if (selectedMod == null || cc?.ModList == null) return;

            int i = cc.ModList.IndexOf(selectedMod);
            if (i == -1) return;

            int newPos = i + moveBy;
            if (newPos < 0) newPos = 0;
            if (newPos >= cc.ModList.Count) newPos = cc.ModList.Count - 1;

            cc.ModList.Remove(selectedMod);
            cc.ModList.Insert(newPos, selectedMod);
            UpdateModListBox();
        }

        private void UpdateModListBox()
        {
            lbModList.Items.Clear();

            if (cc?.ModList == null) return;

            foreach (Mod m in cc.ModList)
                lbModList.Items.Add(m);

            if (selectedMod != null)
            {
                lbModList.SelectedIndex = lbModList.Items.IndexOf(selectedMod);
            }
            cc.UpdateModList();
        }

        private void LbModList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbModList.SelectedItem == null) return;

            selectedMod = (Mod)lbModList.SelectedItem;
            DisplayModInfo(selectedMod);
        }

        private void DisplayModInfo(Mod mod)
        {
            lbModName.Text = mod.title;
            lbModTag.Text = mod.tag;
            lbModId.Text = mod.id;
        }

        private void BtClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LlbSteamPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (selectedMod == null || string.IsNullOrEmpty(selectedMod.id)) return;

            System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=" + selectedMod.id);
        }
    }
}
