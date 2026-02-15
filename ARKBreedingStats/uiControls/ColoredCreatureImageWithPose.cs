using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.SpeciesImages;
using ARKBreedingStats.utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

namespace ARKBreedingStats.uiControls
{
    public class ColoredCreatureImageWithPose : Panel
    {
        private const int ButtonHeight = 24;
        private readonly PictureBox _pb = new PictureBox { Dock = DockStyle.Fill };
        private readonly Button _btPreviousPose = new Button { Text = "←", Dock = DockStyle.Left, AutoSize = true };
        private readonly Button _btNextPose = new Button { Text = "→", Dock = DockStyle.Right, AutoSize = true };
        private readonly Label _lbPose = new Label { TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Height = ButtonHeight };
        private readonly int _imageSize;
        private Species _species;
        private byte[] _colorIds;
        private Sex _sex;
        private string _game;
        private ToolTip _tt;
        /// <summary>
        /// Copy infographic of currently displayed creature to clipboard.
        /// </summary>
        public event Action CopyInfoGraphicToClipboard;

        /// <summary>
        /// Contains species where the pose just changed and images to be updated.
        /// </summary>
        public static readonly HashSet<Species> SpeciesChangedPoses = new HashSet<Species>();

        public ColoredCreatureImageWithPose() : this(256) { }

        public ColoredCreatureImageWithPose(int imageWidth, ToolTip tt = null)
        {
            Width = imageWidth;
            Height = imageWidth + ButtonHeight;
            _imageSize = imageWidth;
            var pButtons = new Panel { Dock = DockStyle.Bottom, Height = ButtonHeight };
            pButtons.Controls.Add(_btPreviousPose);
            pButtons.Controls.Add(_btNextPose);
            pButtons.Controls.Add(_lbPose);
            Controls.Add(pButtons);
            Controls.Add(_pb);
            _btPreviousPose.Click += (s, e) => ChangePose(-1);
            _btNextPose.Click += (s, e) => ChangePose(1);
            _tt = tt ?? new ToolTip();
            _pb.Click += SpeciesPictureBoxClick;
            _tt.SetToolTip(_pb, "Click to copy image to the clipboard\nLeft click: plain image\nRight click: image with color info");
            _tt.SetToolTip(_lbPose, "Some species may have more than one pose, this can be set here.");
        }

        public void SetImage(Bitmap bmp, CreatureImageFile.NeighbourPoseExist neighbourPoseExist)
        {
            _pb.SetImageAndDisposeOld(bmp);
            var poseId = Poses.GetPose(_species);
            _btPreviousPose.Visible = poseId > 0; // neighbourPoseExist.HasFlag(CreatureImageFile.NeighbourPoseExist.Previous);
            _btNextPose.Visible = neighbourPoseExist.HasFlag(CreatureImageFile.NeighbourPoseExist.Next);
            _lbPose.Text = poseId > 0 || neighbourPoseExist != CreatureImageFile.NeighbourPoseExist.None ? $"Pose: {poseId}" : string.Empty;
        }

        public void SetCreatureImage(Species species = null, byte[] colorIds = null, Sex sex = Sex.Unspecified, string game = null)
        {
            _species = species ?? _species;
            _colorIds = colorIds ?? _colorIds;
            _sex = sex == Sex.Unspecified ? _sex : sex;
            _game = game ?? _game;
            CreatureColored.GetColoredCreatureWithCallback(SetImage, this,
                _colorIds, _species, _species?.EnabledColorRegions, _imageSize,
                onlyImage: true, creatureSex: _sex, game: _game);
        }

        public void SetClickEventInfographic(Action a)
        {
            if (a == null) return;
            _tt.SetToolTip(_pb, @"Click to copy infographic of this creature to the clipboard.
With hold Ctrl key and left click: plain image
With hold Ctrl key and right click: image with color info");
            _pb.Click -= SpeciesPictureBoxClick;
            CopyInfoGraphicToClipboard += a;
            _pb.Click += (s, e) =>
            {
                if (Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control))
                    SpeciesPictureBoxClick(s, e);
                else
                    CopyInfoGraphicToClipboard?.Invoke();
            };
        }

        private void SpeciesPictureBoxClick(object sender, EventArgs e)
        {
            if (_pb.Image == null) return;
            if (e is System.Windows.Forms.MouseEventArgs me && me.Button == MouseButtons.Right)
                ClipboardHandler.SetImageWithAlphaToClipboard(CreatureInfoGraphic.GetImageWithColors(_pb.Image, _colorIds, _species));
            else
                ClipboardHandler.SetImageWithAlphaToClipboard(_pb.Image, false);
        }

        private void ChangePose(int poseDelta)
        {
            poseDelta *= Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift) ? 5 : 1;
            var previouslySelectedPose = Poses.GetPose(_species);
            var setPoseTo = Math.Max(0, previouslySelectedPose + poseDelta);
            if (setPoseTo == previouslySelectedPose) return;
            Poses.SetPose(_species, setPoseTo);
            SetCreatureImage();
            SpeciesChangedPoses.Add(_species);
        }
    }
}
