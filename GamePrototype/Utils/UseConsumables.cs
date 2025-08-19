using GamePrototype.Items.ConsumItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Units;

namespace GamePrototype.Utils
{
    public class UseConsumables
    {
        public void UseConsumItem(ConsumItem consumitem, Player player, Weapon weapon = null)
        {
            if (consumitem is HealthPotion healthpotion)
            {
                player.RestoreHealth(healthpotion.HealthRestore);
            }
            else if (consumitem is Grindstone grindstone && weapon != null)
            {
                weapon.Repair(grindstone.DurabilityRestore);
            }
        }
    }
}
