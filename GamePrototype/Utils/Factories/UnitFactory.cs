using GamePrototype.Units;

namespace GamePrototype.Utils.Factories
{
    public abstract class UnitFactory
    {
        public abstract Unit CreatePlayer(string name);
        public abstract Unit CreateEnemy();
    }
}
