using ARKBreedingStats.AsbServer;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.library;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ArkSmartBreeding.Enums;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        private void listenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listenToolStripMenuItem.Checked)
                AsbServerStartListening();
            else AsbServerStopListening();
        }

        private void listenWithNewTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AsbServerStartListening(true);
            listenToolStripMenuItem.Checked = true;
        }

        private void AsbServerStartListening(bool useNewToken = false)
        {
            var progressReporter = new Progress<ProgressReportAsbServer>(AsbServerDataSent);
            if (useNewToken || string.IsNullOrEmpty(Properties.Settings.Default.ExportServerToken))
                Properties.Settings.Default.ExportServerToken = AsbServer.Connection.CreateNewToken();
            Task.Factory.StartNew(() => AsbServer.Connection.StartListeningAsync(progressReporter, Properties.Settings.Default.ExportServerToken));
        }

        private void AsbServerStopListening(bool displayMessage = true)
        {
            if (!AsbServer.Connection.StopListening()) return;
            if (displayMessage)
                SetMessageLabelText($"ASB Server listening stopped using token: {Connection.TokenStringForDisplay(Properties.Settings.Default.ExportServerToken)}", MessageBoxIcon.Error);
        }

        private void currentTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tokenIsSet = !string.IsNullOrEmpty(Properties.Settings.Default.ExportServerToken);
            string message;
            bool isError;
            if (tokenIsSet)
            {
                message = $"Currently {(Connection.IsCurrentlyListening ? string.Empty : "not ")}listening to the server."
                          + " The current token is " + Environment.NewLine + Connection.TokenStringForDisplay(Properties.Settings.Default.ExportServerToken);

                if (utils.ClipboardHandler.SetText(Properties.Settings.Default.ExportServerToken))
                    message += Environment.NewLine + "(token copied to clipboard)";
                isError = false;
            }
            else
            {
                message = "Currently no token set. A token is created once you start listening to the server.";
                isError = true;
            }

            SetMessageLabelText(message, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information,
                clipboardText: Properties.Settings.Default.ExportServerToken, displayPopup: !Properties.Settings.Default.StreamerMode && Properties.Settings.Default.DisplayPopupForServerToken);
        }

        private void sendExampleCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // debug function, sends a test creature to the server
            AsbServer.Connection.SendCreatureData(DummyCreatures.CreateCreature(speciesSelector1.SelectedSpecies), Properties.Settings.Default.ExportServerToken);
        }

        private void sendServerCreatureStatusNeuterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendServerCreatureStatusForSelectedCreature(Connection.ServerCreatureStatusNeuter);
        }

        private void sendServerCreatureStatusDeadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendServerCreatureStatusForSelectedCreature(Connection.ServerCreatureStatusDead);
        }

        private void SendServerCreatureStatusForSelectedCreature(string status)
        {
            // debug function, sends a status change of the selected creature to the server
            Creature cr = null;
            if (listViewLibrary.SelectedIndices.Count > 0)
                cr = (Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag;
            if (cr == null) return;

            // debug function, sends a status change of the selected creature to the server
            AsbServer.Connection.SendCreatureStatus(cr.ArkId, Properties.Settings.Default.ExportServerToken, status);
        }

        /// <summary>
        /// Handle reports from the AsbServer listening, e.g. importing creatures or handle errors.
        /// </summary>
        private void AsbServerDataSent(ProgressReportAsbServer data)
        {
            if (!string.IsNullOrEmpty(data.Message))
            {
                var displayPopup = false;
                var message = data.Message;
                string popupMessage = null;
                var copyToClipboard = !string.IsNullOrEmpty(data.ClipboardText);
                if (copyToClipboard && !utils.ClipboardHandler.SetText(data.ClipboardText))
                    copyToClipboard = false;

                if (!string.IsNullOrEmpty(data.ServerToken))
                {
                    displayPopup = !Properties.Settings.Default.StreamerMode && Properties.Settings.Default.DisplayPopupForServerToken;
                    var tokenInfo = Environment.NewLine
                                    + Connection.TokenStringForDisplay(data.ServerToken)
                                    + (copyToClipboard
                                        ? Environment.NewLine + "(this has been copied to the clipboard)"
                                        : string.Empty);

                    if (displayPopup)
                        popupMessage = message + Environment.NewLine + tokenInfo
                            + Environment.NewLine + Environment.NewLine + "Enable Streamer mode in Settings -> General to mask the token in the future";
                    message += tokenInfo;
                }

                if (listenToolStripMenuItem.Checked == data.StoppedListening)
                    listenToolStripMenuItem.Checked = !data.StoppedListening;

                SetMessageLabelText(message, data.IsError ? MessageBoxIcon.Error : MessageBoxIcon.Information, clipboardText: data.ClipboardText,
                    displayPopup: displayPopup, customPopupText: popupMessage);

                return;
            }

            if (data.SetFlag != CreatureFlags.None)
            {
                // set creature flag
                var cr = _creatureCollection.creatures.FirstOrDefault(c => c.ArkId == data.creatureId);
                if (cr == null)
                {
                    SetMessageLabelText($"No creature found with id {data.creatureId}", MessageBoxIcon.Error);
                    return;
                }

                switch (data.SetFlag)
                {
                    case CreatureFlags.Neutered:
                        SetFlagNeutered(new[] { cr }, true);
                        SetMessageLabelText($"Set {cr.name} to neutered", MessageBoxIcon.Information);
                        _ignoreNextMessageLabel = true; // ignore message of index changed
                        return;
                    case CreatureFlags.Dead:
                        SetCreatureStatus(new[] { cr }, CreatureStatus.Dead);
                        SetMessageLabelText($"Set status of {cr.name} to dead", MessageBoxIcon.Information);
                        _ignoreNextMessageLabel = true; // ignore message of index changed
                        return;
                }

                return;
            }

            string resultText;
            if (string.IsNullOrEmpty(data.ServerHash))
            {
                // import creature
                var creature = ImportExportGun.LoadCreatureFromExportGunJson(data.JsonText, out resultText, out _);
                if (creature == null)
                {
                    SetMessageLabelText(resultText, MessageBoxIcon.Error);
                    return;
                }

                creature.domesticatedAt = DateTime.Now;

                var addCreature = Properties.Settings.Default.OnAutoImportAddToLibrary;
                var gotoLibraryTab = addCreature && Properties.Settings.Default.AutoImportGotoLibraryAfterSuccess;

                DetermineLevelStatusAndSoundFeedback(creature, Properties.Settings.Default.PlaySoundOnAutoImport);
                SetNameOfImportedCreature(creature, null, out _,
                        _creatureCollection.creatures.FirstOrDefault(c => c.guid == creature.guid));

                if (addCreature)
                {
                    data.TaskNameGenerated?.SetResult(new ServerSendName { CreatureName = creature.name, ConnectionToken = data.ServerToken, ExportId = data.SendId });

                    _creatureCollection.MergeCreatureList(new[] { creature }, true);
                    UpdateCreatureParentLinkingSort(goToLibraryTab: gotoLibraryTab);
                }
                else
                {
                    SetCreatureValuesLevelsAndInfoToExtractor(creature);
                }

                if (resultText == null)
                    resultText = $"Received creature from server: {creature}";

                SetMessageLabelText(resultText, MessageBoxIcon.Information);

                if (gotoLibraryTab)
                {
                    tabControlMain.SelectedTab = tabPageLibrary;
                    if (listBoxSpeciesLib.SelectedItem != null &&
                        listBoxSpeciesLib.SelectedItem != creature.Species)
                        listBoxSpeciesLib.SelectedItem = creature.Species;

                    _ignoreNextMessageLabel = true;
                    SelectCreatureInLibrary(creature);
                }
                return;
            }

            // import server settings
            var success = ImportExportGun.ImportServerMultipliersFromJson(_creatureCollection, data.JsonText, data.ServerHash, out resultText);
            SetMessageLabelText(resultText, success ? MessageBoxIcon.Information : MessageBoxIcon.Error, resultText);
        }
    }
}
