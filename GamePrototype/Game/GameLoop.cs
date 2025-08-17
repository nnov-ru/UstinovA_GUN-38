using GamePrototype.Units;
using GamePrototype.Dungeon;
using GamePrototype.Combat;
using GamePrototype.Utils;
namespace GamePrototype.Game
{
    public sealed class GameLoop
    {
        private Unit _player;
        private DungeonRoom _dungeon;
        private readonly CombatManager _combatManager = new CombatManager();
        public void StartGame()
        {
            Initialize();
            Console.WriteLine("You enter the Dungeon");
            StartGameLoop();
        }
        #region Game Loop
        private void Initialize()
        {
            Console.WriteLine("Welcome, player!");
            _dungeon = DungeonBuilder.BuildDungeon();
            Console.WriteLine("Would you please type your name");
            _player = UnitFactoryDemo.CreatePlayer(Console.ReadLine());
            Console.WriteLine($"Welcome again, {_player.Name}");
        }
        private void StartGameLoop()
        {
            var currentroom = _dungeon;
            while (currentroom.IsFinal == false)
            {
                StartRoomEncounter(currentroom, out var success);
                if (!success)
                {
                    Console.WriteLine("Game over !");
                    return;
                }
                DisplayRouteOptions(currentroom);
                while (true)
                {
                    if (Enum.TryParse<Direction>(Console.ReadLine(), out var userinput))
                    {
                        if (currentroom.Rooms.TryGetValue(userinput, out var room))
                        {
                            currentroom = room;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Wrong direction !");
                            DisplayRouteOptions(currentroom);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong input !");
                    }
                }
            }
            Console.WriteLine($"Our congrats, {_player.Name}");
            Console.WriteLine("Result:");
            Console.WriteLine(_player.ToString());
        }
        private void StartRoomEncounter(DungeonRoom currentroom, out bool success)
        {
            success = true;
            if (currentroom.Loot != null)
            {
                _player.AddItemtoInventory(currentroom.Loot);
            }
            if (currentroom.Enemy != null)
            {
                if (_combatManager.StartCombat(_player, currentroom.Enemy) == _player)
                {
                    _player.HandleCombatCompleted();
                    LootEnemy(currentroom.Enemy);
                }
                else
                {
                    success = false;
                }
            }
            void LootEnemy(Unit enemy)
            {
                _player.AddItemfromUnittoInventory(enemy);
            }
        }
        private void DisplayRouteOptions(DungeonRoom currentroom)
        {
            Console.WriteLine("Where would you like to go?");
            foreach (var room in currentroom.Rooms)
            {
                Console.Write($"{room.Key} - {(int)room.Key}\t");
            }
        }
        #endregion
    }
}