using ARKBreedingStats.Discord;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private readonly HelenaWalkerDiscordClient _discordClient;

        private ISocketMessageChannel _discordChannel;

        private bool IsDiscordClientReady()
            => _discordClient.ConnectionState == ConnectionState.Connected
            && _discordClient != null;

        private async Task LoadDiscordClient()
        {
            if (_discordClient.LoginState == LoginState.LoggedIn)
                return;

            // Subscribing to client events, so that we may receive them whenever they're invoked.
            _discordClient.Ready += ReadyAsync;
            //_discordClient.Log += LogAsync;
            //_discordClient.MessageReceived += MessageReceivedAsync;
            //_discordClient.InteractionCreated += InteractionCreatedAsync;


            // Tokens should be considered secret data, and never hard-coded.
            string discordBotToken = Properties.Settings.Default.DiscordBotToken;

            if (!string.IsNullOrWhiteSpace(discordBotToken))
            {
                await _discordClient.LoginAsync(TokenType.Bot, discordBotToken);
                await _discordClient.StartAsync();
            }
        }

        private async Task ReadyAsync()
        {
            ulong discordChannelId = Properties.Settings.Default.DiscordChannelId;

            _discordChannel = (ISocketMessageChannel)await _discordClient
                .GetChannelAsync(discordChannelId);

            if (_discordChannel != null)
            {
                buttonSendImageToDiscord.Show();
                toolStripCopyInfographicsToDiscordToolStripMenuItem.Visible = true;
            }
        }


        public Stream ImageToStream(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            return ms;
        }

        private void SendImageToDiscord(System.Drawing.Image image, ISocketMessageChannel channel = null)
        {
            if (image == null)
                return;

            if (channel == null)
                channel = _discordChannel;

            if (IsDiscordClientReady())
            {
                using (Stream stream = ImageToStream(image, System.Drawing.Imaging.ImageFormat.Png))
                {
                    channel.SendFileAsync(stream, "image.png").Wait();
                }
            }
        }

    }
}
