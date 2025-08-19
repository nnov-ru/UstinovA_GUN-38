using GamePrototype.Units;
using GamePrototype.Dungeon;
using GamePrototype.Combat;
using GamePrototype.Utils;
using GamePrototype.Utils.Factories;
using GamePrototype.Items.EquipItems;
using GamePrototype.Items.ConsumItems;
namespace GamePrototype.Game
{
    public sealed class GameLoop
    {
        private Unit _player;
        private DungeonRoom _dungeon;
        private readonly CombatManager _combatManager = new CombatManager();
        private UseConsumables _useconsumables;
        private Difficulty _difficulty;
        public UseConsumables UseConsumables
        {
            get => _useconsumables ??= new UseConsumables();
            set => _useconsumables = value;
        }
        public void StartGame()
        {
            Initialize();
            Console.WriteLine("Entering the Dungeon...");
            StartGameLoop();
        }
        #region Game Loop
        private void Initialize()
        {
            _difficulty = ChooseDifficulty();
            var (unitfactory, dungeonbuilder) = GameFactory.GetFactories(_difficulty);

            Console.WriteLine("Welcome, player!");
            _dungeon = dungeonbuilder.BuildDungeon();
            Console.WriteLine("Enter your name");
            string input = Console.ReadLine();
            Console.WriteLine($"Hello {input}");
            _player = unitfactory.CreatePlayer(input);
        }
        private Difficulty ChooseDifficulty()
        {
            Console.WriteLine("Choose Difficulty Level:");
            Console.WriteLine("1 - Easy - More Health, Weaker Enemies");
            Console.WriteLine("2 - Hard - Less Resources, Stronger Enemies");

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "1") return Difficulty.Easy;
                if (input == "2") return Difficulty.Hard;
                Console.WriteLine("Incorrect input! Please input 1 or 2 :");
            }
        }
        private void StartGameLoop()
        {
            var currentroom = _dungeon;
            while (currentroom.IsFinal != null && !currentroom.IsFinal)
            {
                Console.WriteLine($"\n===Entered room : {currentroom.Name}...===");
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
            Console.WriteLine($"\n===Our congrats, {_player.Name} : you reached the Exit!===");
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
            if (currentroom.Name == "Here is a useful Grindstone")
            {
                Console.WriteLine($"Grindstone is used to repair your weapon automatically, when the Weapon is damaged.");
                if (_player is Player user1 && user1.EquippedWeapon is Weapon weapon && weapon.Durability < weapon.MaxDurability)
                {
                    _player.HandleCombatCompleted();
                }
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