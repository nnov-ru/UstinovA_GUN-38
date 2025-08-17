using GamePrototype.Items.ConsumItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Units;
namespace GamePrototype.Utils
{
    public class UnitFactoryDemo
    {
        public static Unit CreatePlayer(string name)
        {
            var player = new Player(name, 30, 30, 6);
            player.AddItemtoInventory(new Weapon(10, 15, "Sword"));
            player.AddItemtoInventory(new Armor(10, 15, "Armor"));
            player.AddItemtoInventory(new HealthPotion("Potion"));
            return player;
        }
        public static Unit CreateGoblinEnemy()
        { 
            var goblin = new Goblin(GameConstants.Goblin, 18, 18, 2);
            goblin.AddItemtoInventory(new Gold());
            return goblin;
        }
    }
}
