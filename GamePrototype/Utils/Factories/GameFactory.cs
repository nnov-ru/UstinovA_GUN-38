using GamePrototype.Utils.Builders;
using GamePrototype.Utils;
namespace GamePrototype.Utils.Factories
{
    public static class GameFactory
    {
        public static (UnitFactory unitfactory, DungeonBuilder dungeonbuilder) GetFactories(Difficulty difficulty)
        {
            UnitFactory unitfactory = difficulty switch
            {
                Difficulty.Easy => new UnitFactoryEasy(),
                Difficulty.Hard => new UnitFactoryHard(),
                _ => throw new ArgumentException("Unknown difficulty level")
            };
            DungeonBuilder dungeonbuilder = difficulty switch
            {
                Difficulty.Easy => new DungeonBuilderEasy(unitfactory),
                Difficulty.Hard => new DungeonBuilderHard(unitfactory),
                _ => throw new ArgumentException("Unknown difficulty level")
            };
            return (unitfactory, dungeonbuilder);
        }
    }
}
