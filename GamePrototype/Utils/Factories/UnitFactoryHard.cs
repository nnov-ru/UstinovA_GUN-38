using GamePrototype.Items.ConsumItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Units;
namespace GamePrototype.Utils.Factories
{
    public class UnitFactoryHard : UnitFactory
    {
        public override Unit CreatePlayer(string name)
        {
            var player = new Player(name, 15, 15, 5);
            player.AddItemtoInventory(new Weapon(5, 10, "Short Sword"));
            return player;
        }
        public override Unit CreateEnemy()
        {
            var goblin = new Goblin(GameConstants.Goblin, 20,20,4);
            goblin.AddItemtoInventory(new Weapon(20,60,"Poisoned Staff"));
            goblin.AddItemtoInventory(new Armor(5,10,"Wooden Plates"));
            return goblin;
        }
    }
}
