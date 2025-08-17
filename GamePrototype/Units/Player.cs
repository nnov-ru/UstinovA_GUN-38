using System.Text;
using GamePrototype.Items;
using GamePrototype.Items.ConsumItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Utils;
namespace GamePrototype.Units
{
    public sealed class Player : Unit
    {
        private readonly Dictionary<EquipSlot, EquipItem> _equipment = new();
        public Player(string name, int health, uint maxhealth, uint basedamage) : base(name, health, maxhealth, basedamage)
        {
        }
        public override uint GetUnitDamage()
        {
            if (_equipment.TryGetValue(EquipSlot.Weapon, out var item) && item is Weapon weapon)
            { 
                return BaseDamage + weapon.Damage;
            }
            return BaseDamage;
        }
        public override void HandleCombatCompleted()
        {
            var items = Inventory.Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is ConsumItem consumitem)
                {
                    if (Health < MaxHealth)
                    {
                        UseConsumItem(consumitem);
                        Inventory.TryRemove(items[i]);
                    }
                }
            }
        }
        public override void AddItemtoInventory(Item item)
        {
            if (item is EquipItem equipitem && _equipment.TryAdd(equipitem.Slot, equipitem))
            {
                return;
            }
            base.AddItemtoInventory(item);
        }
        private void UseConsumItem(ConsumItem consumitem)
        {
            if (consumitem is HealthPotion healthpotion)
            {
                Health = Math.Min((int)MaxHealth, Health + (int)healthpotion.HealthRestore);
            }
        }
        protected override uint CalculateAppliedDamage(uint damage)
        {
            if (_equipment.TryGetValue(EquipSlot.Armor, out var item) && item is Armor armor)
            {
                damage -= (uint)(damage * (armor.Defence / 100f));
            }
            return damage;
        }
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Name);
            builder.AppendLine($"Health: {Health} / {MaxHealth}");
            builder.AppendLine("Loot Held:");
            var items = Inventory.Items;
            for (int i = 0; i < items.Count; i++)
                {
                builder.AppendLine($"{items[i].Name} : {items[i].Amount}");
                }
            return builder.ToString();
        }
    }
}
