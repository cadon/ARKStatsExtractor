using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.SpeciesImages
{
    internal class CreatureImageDisplayWithPose : PictureBox
    {
        private readonly int _imageSize;
        private Species _species;
        private byte[] _colorIds;
        private Sex _sex;
        private string _game;
        private readonly ToolTip _tt;
        private readonly Button _buttonChangePose;
        /// <summary>
        /// Copy infographic of currently displayed creature to clipboard.
        /// </summary>
        public event Action CopyInfoGraphicToClipboard;

        public CreatureImageDisplayWithPose(Button buttonChangePose, int imageSize, ToolTip tt = null)
        {
            _buttonChangePose = buttonChangePose;
            Width = imageSize;
            Height = imageSize;
            _imageSize = imageSize;
            _tt = tt ?? new ToolTip();
            _tt.SetToolTip(buttonChangePose, "Click to change the pose of this creature.");
            buttonChangePose.Click += (s, e) => ChangePose();
            this.Click += (s, e) => CopyInfoGraphicToClipboard?.Invoke();
        }

        private void ChangePose()
        {
            if (_buttonChangePose.Tag is not int setPoseTo || setPoseTo < 0)
                return;

            Poses.SetPose(_species, setPoseTo);
            SetCreatureImage();
            Poses.SpeciesChangedPoses.Add(_species);
        }

        private void SetImage(Bitmap bmp, CreatureImageFile.NeighbourPoseExist neighbourPoseExist)
        {
            this.SetImageAndDisposeOld(bmp);
            if (bmp == null)
            {
                _buttonChangePose.Tag = -1;
                _buttonChangePose.Visible = false;
                _tt.SetToolTip(this, null);
                return;
            }
            _tt.SetToolTip(this, CreatureColored.RegionColorInfo(_species, _colorIds)
                                                  + "\n\nClick to copy creature infos as image to the clipboard");
            var poseId = Poses.GetPose(_species);
            var otherPosesExist = poseId > 0 || neighbourPoseExist.HasFlag(CreatureImageFile.NeighbourPoseExist.Next);
            _buttonChangePose.Visible = otherPosesExist;
            // set Tag to next pose that should be set when button clicked
            _buttonChangePose.Tag = !otherPosesExist ? -1 :
                neighbourPoseExist.HasFlag(CreatureImageFile.NeighbourPoseExist.Next) ? poseId + 1 : 0;
        }

        public void SetCreatureImage(Species species = null, byte[] colorIds = null, Sex sex = Sex.Unspecified, string game = null)
        {
            _species = species ?? _species;
            _colorIds = colorIds ?? _colorIds;
            _sex = sex == Sex.Unspecified ? _sex : sex;
            _game = game ?? _game;
            SetImage(null, CreatureImageFile.NeighbourPoseExist.None); // clear current image
            CreatureColored.GetColoredCreatureWithCallback(SetImage, this,
                _colorIds, _species, _species?.EnabledColorRegions, _imageSize,
                onlyImage: true, creatureSex: _sex, game: _game);
        }
    }
}
