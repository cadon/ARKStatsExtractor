namespace ASB_Updater
{
    interface IUpdater
    {

        bool hasEXE();

        string getEXE();

        string lastError();

        int getProgress();

        bool fetch();

        bool parse();

        bool download();

        bool extract(string workingDirectory);

        bool cleanup();

    }
}
