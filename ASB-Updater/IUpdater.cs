namespace ASB_Updater
{
    interface IUpdater
    {
        string LastError();

        int GetProgress();

        bool Fetch();

        bool Parse();

        bool Download();

        bool Extract(string workingDirectory);

        bool Cleanup();

    }
}
