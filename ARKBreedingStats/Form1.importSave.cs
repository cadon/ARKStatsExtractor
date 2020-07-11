using ARKBreedingStats.miscClasses;
using ARKBreedingStats.settings;
using FluentFTP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        private void SavegameImportClick(object sender, EventArgs e)
        {
            RunSavegameImport((ATImportFileLocation)((ToolStripMenuItem)sender).Tag);
        }

        /// <summary>
        /// Imports the creatures from the given savegame. ftp is possible.
        /// </summary>
        /// <param name="atImportFileLocation"></param>
        private async void RunSavegameImport(ATImportFileLocation atImportFileLocation)
        {
            try
            {
                string workingCopyfilename = Properties.Settings.Default.savegameExtractionPath;

                // working dir not configured? use temp dir
                // luser configured savegame folder as working dir? use temp dir instead
                if (string.IsNullOrWhiteSpace(workingCopyfilename) ||
                        Path.GetDirectoryName(atImportFileLocation.FileLocation) == workingCopyfilename)
                {
                    workingCopyfilename = Path.GetTempPath();
                }


                if (Uri.TryCreate(atImportFileLocation.FileLocation, UriKind.Absolute, out var uri)
                    && uri.Scheme != "file")
                {
                    switch (uri.Scheme)
                    {
                        case "ftp":
                            workingCopyfilename = await CopyFtpFileAsync(uri, atImportFileLocation.ConvenientName, workingCopyfilename);
                            if (workingCopyfilename == null)
                                // the user didn't enter credentials
                                return;
                            break;
                        default:
                            throw new Exception($"Unsupported uri scheme: {uri.Scheme}");
                    }
                }
                else
                {
                    workingCopyfilename = Path.Combine(workingCopyfilename, Path.GetFileName(atImportFileLocation.FileLocation));
                    File.Copy(atImportFileLocation.FileLocation, workingCopyfilename, true);
                }

                await ImportSavegame.ImportCollectionFromSavegame(_creatureCollection, workingCopyfilename, atImportFileLocation.ServerName);

                UpdateParents(_creatureCollection.creatures);

                foreach (var creature in _creatureCollection.creatures)
                {
                    creature.RecalculateAncestorGenerations();
                }

                UpdateIncubationParents(_creatureCollection);

                // update UI
                SetCollectionChanged(true);
                UpdateCreatureListings();

                if (_creatureCollection.creatures.Any())
                    tabControlMain.SelectedTab = tabPageLibrary;

                // reapply last sorting
                listViewLibrary.Sort();

                UpdateTempCreatureDropDown();

                // if unknown mods are used in the savegame-file and the user wants to load the missing mod-files, do it
                if (_creatureCollection.ModValueReloadNeeded
                    && LoadModValuesOfCollection(_creatureCollection, true, true))
                    SetCollectionChanged(true);
            }
            catch (Exception ex)
            {
                string message = ex.Message
                       + "\n\nException in " + ex.Source
                       + "\n\nMethod throwing the error: " + ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name
                       + "\n\nStackTrace:\n" + ex.StackTrace
                       + (ex.InnerException != null ? "\n\nInner Exception:\n" + ex.InnerException.Message : string.Empty)
                       ;
                MessageBox.Show($"An error occured while importing. Message: \n\n{message}",
                        "Import Error", MessageBoxButtons.OK);
            }
        }

        private async Task<string> CopyFtpFileAsync(Uri ftpUri, string serverName, string workingCopyFolder)
        {
            var credentialsByServerName = LoadSavedCredentials();
            credentialsByServerName.TryGetValue(serverName, out var credentials);

            var dialogText = $"Ftp Credentials for {serverName}";

            using (var cancellationTokenSource = new CancellationTokenSource())
            using (var progressDialog = new FtpProgressForm(cancellationTokenSource))
            {
                while (true)
                {
                    if (credentials == null)
                    {
                        // get new credentials
                        using (var dialog = new FtpCredentialsForm { Text = dialogText })
                        {
                            if (dialog.ShowDialog(this) == DialogResult.Cancel)
                            {
                                return null;
                            }

                            credentials = dialog.Credentials;

                            if (dialog.SaveCredentials)
                            {
                                credentialsByServerName[serverName] = credentials;
                                Properties.Settings.Default.SavedFtpCredentials = Encryption.Protect(JsonConvert.SerializeObject(credentialsByServerName));
                                Properties.Settings.Default.Save();
                            }
                        }
                    }
                    var client = new FtpClient(ftpUri.Host, ftpUri.Port, credentials.Username, credentials.Password);

                    try
                    {
                        progressDialog.StatusText = $"Authenticating";
                        if (!progressDialog.Visible)
                            progressDialog.Show(this);

                        // TODO
                        // cancel token doesn't work correctly, instead of throwing 
                        // TaskCanceledException
                        // on cancelling it throws
                        // Cannot access a disposed object. Object name: 'System.Net.Sockets.Socket'.
                        await client.ConnectAsync(token: cancellationTokenSource.Token);

                        progressDialog.StatusText = $"Finding most recent file";
                        await Task.Yield();

                        var ftpPath = ftpUri.AbsolutePath;
                        var lastSegment = ftpUri.Segments.Last();
                        if (lastSegment.Contains("*"))
                        {
                            var mostRecentlyModifiedMatch = await GetLastModifiedFileAsync(client, ftpUri, cancellationTokenSource.Token);
                            if (mostRecentlyModifiedMatch == null)
                            {
                                throw new Exception($"No file found matching pattern '{lastSegment}'");
                            }

                            ftpPath = mostRecentlyModifiedMatch.FullName;
                        }

                        var fileName = Path.GetFileName(ftpPath);

                        progressDialog.FileName = fileName;
                        progressDialog.StatusText = $"Downloading {fileName}";
                        await Task.Yield();

                        var filePath = Path.Combine(workingCopyFolder, Path.GetFileName(ftpPath));
                        await client.DownloadFileAsync(filePath, ftpPath, FtpLocalExists.Overwrite, FtpVerify.Retry, progressDialog, token: cancellationTokenSource.Token);
                        await Task.Delay(500, cancellationTokenSource.Token);

                        if (filePath.EndsWith(".gz"))
                        {
                            progressDialog.StatusText = $"Decompressing {fileName}";
                            await Task.Yield();

                            filePath = await DecompressGZippedFileAsync(filePath, cancellationTokenSource.Token);
                        }

                        return filePath;
                    }
                    catch (FtpAuthenticationException ex)
                    {
                        // if auth fails, clear credentials, alert the user and loop until the either auth succeeds or the user cancels
                        progressDialog.StatusText = $"Authentication failed: {ex.Message}";
                        credentials = null;
                        await Task.Delay(1000);
                    }
                    catch (OperationCanceledException)
                    {
                        client?.Dispose();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        if (progressDialog.IsDisposed)
                        {
                            client?.Dispose();
                            return null;
                        }
                        progressDialog.StatusText = $"Unexpected error: {ex.Message}";
                    }
                    finally
                    {
                        client?.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Loads the encrypted ftp crednetials from settings, decrypts them, then returns them as a hostname to credentials dictionary
        /// </summary>
        private static Dictionary<string, FtpCredentials> LoadSavedCredentials()
        {
            try
            {
                var savedCredentials = Encryption.Unprotect(Properties.Settings.Default.SavedFtpCredentials);

                if (!string.IsNullOrEmpty(savedCredentials))
                {
                    var savedDictionary = JsonConvert.DeserializeObject<Dictionary<string, FtpCredentials>>(savedCredentials);

                    // Ensure that the resulting dictionary is case insensitive on hostname
                    return new Dictionary<string, FtpCredentials>(savedDictionary, StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while loading saved ftp credentials. Message: \n\n{ex.Message}",
                        $"Settings Error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK);
            }

            return new Dictionary<string, FtpCredentials>(StringComparer.OrdinalIgnoreCase);
        }

        private async Task<string> DecompressGZippedFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var newFileName = filePath.Remove(filePath.Length - 3);

            using (var originalFileStream = File.OpenRead(filePath))
            using (var decompressedFileStream = File.Create(newFileName))
            using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
            {
                await decompressionStream.CopyToAsync(decompressedFileStream, 81920, cancellationToken);
            }

            return newFileName;
        }

        public async Task<FtpListItem> GetLastModifiedFileAsync(FtpClient client, Uri ftpUri, CancellationToken cancellationToken)
        {
            var folderUri = new Uri(ftpUri, ".");
            var listItems = await client.GetListingAsync(folderUri.AbsolutePath, cancellationToken);

            //  Turn the wildcard into a regex pattern   "super*.foo" ->  "^super.*?\.foo$"
            var nameRegex = new Regex("^" + Regex.Escape(ftpUri.Segments.Last()).Replace(@"\*", ".*?") + "$");

            return listItems
                .OrderByDescending(x => x.Modified)
                .FirstOrDefault(x => nameRegex.IsMatch(x.Name));
        }
    }
}
