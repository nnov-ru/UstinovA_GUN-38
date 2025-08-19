using GamePrototype.Items.ConsumItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Units;
namespace GamePrototype.Utils.Factories
{
    public class UnitFactoryEasy : UnitFactory
    {
        public override Unit CreatePlayer(string name)
        {
            var player1 = new Player(name, 30, 30, 6);
            player1.AddItemtoInventory(new Weapon(10, 15, "Sword"));
            player1.AddItemtoInventory(new Armor(5, 15, "Shield"));
            player1.AddItemtoInventory(new HealthPotion("Healing Potion"));
            player1.AddItemtoInventory(new Helmet(3, 10, "Helmet"));
            return player1;
        }
        public override Unit CreateEnemy()
        { 
            var goblin = new Goblin(GameConstants.Goblin, 18, 18, 2);
            goblin.AddItemtoInventory(new Gold());
            goblin.AddItemtoInventory(new Weapon(10, 30, "Staff"));
            goblin.AddItemtoInventory(new RangeWeapon(5, 8, "Sling"));
            return goblin;
        }
    }
}
