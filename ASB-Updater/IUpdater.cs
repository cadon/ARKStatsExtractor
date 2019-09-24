namespace ASB_Updater
{
    interface IUpdater
    {

        bool HasEXE();

        string GetEXE();

        string LastError();

        int GetProgress();

        bool Fetch();

        bool Parse();

        bool Download();

        bool Extract(string workingDirectory);

        bool Cleanup();

    }
}
