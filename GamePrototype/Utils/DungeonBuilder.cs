using GamePrototype.Dungeon;
using GamePrototype.Items.ConsumItems;

namespace GamePrototype.Utils
{
    public static class DungeonBuilder
    {
        public static DungeonRoom BuildDungeon()
        {
            var entrance = new DungeonRoom("Entrance");
            var monsterroom = new DungeonRoom("Monster", UnitFactoryDemo.CreateGoblinEnemy());
            var emptyroom = new DungeonRoom("Empty");
            var lootroom = new DungeonRoom("Loot1", new Gold());
            var lootstoneroom = new DungeonRoom("Loot2", new Grindstone("Stone"));
            var finalroom = new DungeonRoom("Final");

            entrance.TrySetDirection(Direction.right, monsterroom);
            entrance.TrySetDirection(Direction.forward, emptyroom);

            monsterroom.TrySetDirection(Direction.forward, lootroom);
            monsterroom.TrySetDirection(Direction.left, emptyroom);

            emptyroom.TrySetDirection(Direction.forward, lootstoneroom);

            lootstoneroom.TrySetDirection(Direction.right, finalroom);
            lootroom.TrySetDirection(Direction.left, finalroom);

            return entrance;
        }
    }
}
