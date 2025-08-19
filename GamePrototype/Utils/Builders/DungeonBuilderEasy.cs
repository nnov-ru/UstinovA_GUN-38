using GamePrototype.Dungeon;
using GamePrototype.Items.ConsumItems;
using GamePrototype.Utils.Factories;

namespace GamePrototype.Utils.Builders
{
    public class DungeonBuilderEasy : DungeonBuilder
    {
        public DungeonBuilderEasy(UnitFactory unitfactory) : base(unitfactory) { }
        public override DungeonRoom BuildDungeon()
        {
            var entrance = new DungeonRoom("Entrance");
            var monsterroom = new DungeonRoom("Monster Room", _unitfactory.CreateEnemy());
            var lootroom = new DungeonRoom("There's a Gold Coin here!", new Gold());
            var finalroom = new DungeonRoom("Final");

            entrance.TrySetDirection(Direction.right, monsterroom);
            entrance.TrySetDirection(Direction.forward, lootroom);

            monsterroom.TrySetDirection(Direction.forward, finalroom);
            monsterroom.TrySetDirection(Direction.left, lootroom);

            lootroom.TrySetDirection(Direction.right, finalroom);

            return entrance;
        }
    }
}
