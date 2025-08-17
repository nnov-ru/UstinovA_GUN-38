using GamePrototype.Units;
using GamePrototype.Items;

namespace GamePrototype.Dungeon
{
    public sealed class DungeonRoom
    {
        public readonly string Name;
        public readonly Unit Enemy;
        public readonly Item Loot;
        public readonly Dictionary<Direction, DungeonRoom> Rooms = new();
        public bool IsFinal => Rooms.Count == 0;

        public DungeonRoom(string name) => Name = name;
        public DungeonRoom(string name, Unit enemy) 
        {
            Name = name;
            Enemy = enemy;
        }
        public DungeonRoom(string name, Item loot) 
        {
            Name = name;
            Loot = loot;
        }
        public bool TrySetDirection(Direction direction, DungeonRoom room)
        {
            if (Rooms.ContainsKey(direction))
            {
                Console.WriteLine($"Room {Name} has been already quitted to its {direction.ToString()}");
                return false;
            }
            Rooms.Add(direction, room);
            return true;
        }
    }
}
