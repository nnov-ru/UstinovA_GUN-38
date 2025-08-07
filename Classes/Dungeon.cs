using Structures;

namespace Classes
{
    internal class Dungeon
    {
        private Room[] _rooms;
        public Dungeon() 
        {
            _rooms = new Room[]
            {
                new Room(new Unit("Roman", new Interval(5, 15)), new Weapon("Spear", new Interval(10, 20))),
                new Room(new Unit("Early Christian", new Interval(3, 12)), new Weapon("Sling", new Interval(8, 18))),
                new Room(new Unit("Celt", new Interval(2, 10)), new Weapon("Smoke Bomb", new Interval(5, 15)))
            };
        }

        public void ShowRooms()
        {
            for (int i = 0; i < _rooms.Length; i++)
            { 
                var room = _rooms[i];
                Console.WriteLine($"Unit of Room {i + 1}: "+room.Unit.Name);
                Console.WriteLine($"Weapon of Room {i + 1}: "+room.Weapon.Name);
                Console.WriteLine("-");
            }
        }
    }
}
