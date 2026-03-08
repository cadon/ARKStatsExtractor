namespace ARKBreedingStats.AsbServer
{
    /// <summary>
    /// Contains properties for sending a creature name via server connection.
    /// </summary>
    internal class ServerSendName
    {
        public string CreatureName;
        public string ConnectionToken;
        public string ExportId;
    }
}
