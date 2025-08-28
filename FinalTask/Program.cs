using FinalTask.SaveLoad;
using FinalTask.CasinoMechanics;

namespace FinalTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var saveLoadService = new FileSystemSaveLoadService("Saved Games");
            var casino = new Casino(saveLoadService);

            casino.StartGame();
        }
    }
}
