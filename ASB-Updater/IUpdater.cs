namespace ASB_Updater
{
    interface IUpdater
    {
        string LastError();

        int GetProgress();

        bool Fetch();

        bool Parse();

        bool Check(string exectuablePath);

        bool Download();

        bool Extract(string workingDirectory, bool useLocalAppDataForDataFiles);

        bool Cleanup();

    }
}
