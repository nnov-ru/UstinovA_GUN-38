using GamePrototype.Dungeon;
using GamePrototype.Utils.Factories;

namespace GamePrototype.Utils.Builders
{
    public abstract class DungeonBuilder
    {
        protected readonly UnitFactory _unitfactory;
        public DungeonBuilder(UnitFactory unitfactory)
        {
            _unitfactory = unitfactory;
        }

        public abstract DungeonRoom BuildDungeon();
    }
}
