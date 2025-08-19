using GamePrototype.Dungeon;
using GamePrototype.Items.ConsumItems;
using GamePrototype.Utils.Factories;

namespace GamePrototype.Utils.Builders
{
    public class DungeonBuilderHard : DungeonBuilder
    {
        public DungeonBuilderHard(UnitFactory unitfactory) : base(unitfactory) { }
        public override DungeonRoom BuildDungeon()
        {
            var entrance = new DungeonRoom("Entrance");
            var monsterroom = new DungeonRoom("Monster Room", _unitfactory.CreateEnemy());
            var emptyroom = new DungeonRoom("Empty");
            var lootroom = new DungeonRoom("There's a Gold Coin here!", new Gold());
            var lootstoneroom = new DungeonRoom("Here is a useful Grindstone", new Grindstone("Grindstone"));
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
