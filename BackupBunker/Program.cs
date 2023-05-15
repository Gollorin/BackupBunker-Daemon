namespace BackupBunker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StartManager startManager = new StartManager();
            startManager.StartAsync();

            while (true)
            {

            }
        }
    }
}