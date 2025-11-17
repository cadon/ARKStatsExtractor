using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.SpeciesImages
{
    public partial class ImagePackSelection : Form
    {
        public ImagePackSelection()
        {
            InitializeComponent();

            LbAvailablePacks.Items.AddRange(ImageCollections.ImageManifests.Values.ToArray());
            var enabledPacks = Properties.Settings.Default.SpeciesImagesUrls?
                .Select(packId => packId != null && ImageCollections.ImageManifests.TryGetValue(packId, out var im) ? im : null)
                .Where(p => p != null)
                .ToArray();
            if (enabledPacks?.Any() == true)
                LbEnabledPacks.Items.AddRange(enabledPacks);
            DisplayInfo(null);

            FormClosing += ImagePackSelection_FormClosing;
        }

        private void ImagePackSelection_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                Properties.Settings.Default.SpeciesImagesUrls =
                    LbEnabledPacks.Items.Cast<ImagesManifest>().Select(m => m.Id).ToArray();
        }

        private void LbAvailablePacks_SelectedIndexChanged(object sender, EventArgs e) => DisplayInfo((sender as ListBox)?.SelectedItem as ImagesManifest);

        private void LbEnabledPacks_SelectedIndexChanged(object sender, EventArgs e) => DisplayInfo((sender as ListBox)?.SelectedItem as ImagesManifest);

        private void DisplayInfo(ImagesManifest m)
        {
            LbPackName.Text = m?.Name;
            LbCreators.Text = m?.Creator;
            LbDescription.Text = m?.Description;
            LbSource.Text = m?.Url;
            LLFolder.Text = m == null ? string.Empty : $"Show local folder in explorer: {m.FolderName}";
            if (m != null)
                LLFolder.Tag = Path.GetDirectoryName(ImageCollections.ManifestFilePathOfPack(m.FolderName));
        }

        private void LbAvailablePacks_DoubleClick(object sender, EventArgs e) => AddPack((sender as ListBox)?.SelectedItem as ImagesManifest);

        private void LbEnabledPacks_DoubleClick(object sender, EventArgs e) => RemovePack((sender as ListBox)?.SelectedItem as ImagesManifest);

        private void BtAdd_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == 0)
                AddPack(LbAvailablePacks.SelectedItem as ImagesManifest);
            else
            {
                foreach (ImagesManifest m in LbAvailablePacks.Items)
                    AddPack(m);
            }
        }

        private void BtRemove_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == 0)
                RemovePack(LbEnabledPacks.SelectedItem as ImagesManifest);
            else
                LbEnabledPacks.Items.Clear();
        }

        private void BtRemoveAll_Click(object sender, EventArgs e) => LbEnabledPacks.Items.Clear();

        private void BtMoveUp_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == 0)
                MovePack(LbEnabledPacks.SelectedItem as ImagesManifest, -1);
            else
                MovePack(LbEnabledPacks.SelectedItem as ImagesManifest, int.MinValue);
        }

        private void BtMoveDown_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == 0)
                MovePack(LbEnabledPacks.SelectedItem as ImagesManifest, 1);
            else
                MovePack(LbEnabledPacks.SelectedItem as ImagesManifest, int.MaxValue);
        }

        private void AddPack(ImagesManifest imagePack)
        {
            if (imagePack == null
                || LbEnabledPacks.Items.Contains(imagePack))
                return;
            LbEnabledPacks.Items.Add(imagePack);
            LbEnabledPacks.SelectedIndex = LbEnabledPacks.Items.Count - 1;
        }

        private void RemovePack(ImagesManifest imagePack)
        {
            if (imagePack == null) return;
            LbEnabledPacks.Items.Remove(imagePack);
        }

        private void MovePack(ImagesManifest imagesManifest, int deltaPos)
        {
            if (imagesManifest == null || deltaPos == 0) return;

            var currentIndex = LbEnabledPacks.Items.IndexOf(imagesManifest);
            if (currentIndex == -1) return;

            var newIndex = currentIndex + deltaPos;
            if (newIndex < 0) newIndex = 0;
            if (newIndex >= LbEnabledPacks.Items.Count) newIndex = LbEnabledPacks.Items.Count - 1;
            if (newIndex == currentIndex) return;

            LbEnabledPacks.Items.RemoveAt(currentIndex);
            LbEnabledPacks.Items.Insert(newIndex, imagesManifest);
            LbEnabledPacks.SelectedIndex = newIndex;
        }

        private void LLFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender is LinkLabel ll
                && ll.Tag is string folderPath)
            {
                FileService.OpenFolderInExplorer(folderPath);
            }
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
