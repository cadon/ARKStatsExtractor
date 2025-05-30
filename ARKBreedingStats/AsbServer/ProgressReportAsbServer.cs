﻿using System.Threading.Tasks;
using ARKBreedingStats.Library;

namespace ARKBreedingStats.AsbServer
{
    /// <summary>
    /// Info of progress reports while listening to an AsbServer.
    /// </summary>
    internal class ProgressReportAsbServer
    {
        public string JsonText;
        public string ServerHash;
        public string Message;
        public string ServerToken;
        /// <summary>
        /// Identification used to send an answer to the user.
        /// </summary>
        public string SendId;
        public bool IsError;
        public bool StoppedListening;
        public string ClipboardText;
        public TaskCompletionSource<ServerSendName> TaskNameGenerated;
        public CreatureFlags SetFlag;
        public long creatureId;
    }
}
