using System.Runtime.Intrinsics.X86;
using System.Text;
using GamePrototype.Dungeon;
using GamePrototype.Items;
using GamePrototype.Items.ConsumItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Utils;
using GamePrototype.Game;
namespace GamePrototype.Units
{
    public sealed class Player : Unit
    {
        private UseConsumables _useconsumables; 
        private readonly Dictionary<EquipSlot, EquipItem> _equipment = new();
        public Player(string name, int health, uint maxhealth, uint basedamage) : base(name, health, maxhealth, basedamage)
        {
        }
        public UseConsumables UseConsumables
        { 
            get => _useconsumables ??= new UseConsumables();
            set => _useconsumables = value;
        }
        public override uint DoUnitDamage()
        {
            if (_equipment.TryGetValue(EquipSlot.Weapon, out var item) && item is Weapon weapon)
            { 
                return BaseDamage + weapon.Damage;
            }
            return BaseDamage;
        }
        public override void HandleCombatCompleted()
        {
            bool weaponrepaired = false;
            bool healthrestored = false;
            var items = Inventory.Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is not ConsumItem consumitem) continue;
                
                if (!weaponrepaired && consumitem is Grindstone &&
                        _equipment.TryGetValue(EquipSlot.Weapon, out var item) && item is Weapon weapon && weapon.Durability < weapon.MaxDurability)
                {
                            Console.WriteLine($"{weapon.Name} has durability of {weapon.Durability} / {weapon.MaxDurability}.\nAs you have Grindstone - it can be very useful for repair!");
                            UseConsumables.UseConsumItem(consumitem, this, weapon);
                            Inventory.TryRemove(items[i]);
                            weaponrepaired = true;
                            Console.WriteLine($"{weapon.Name} has durability of {weapon.Durability} / {weapon.MaxDurability} now. Grindstone has run out!");
                            continue;
                }
                if (!healthrestored && consumitem is HealthPotion &&
                    Health < MaxHealth)
                {
                    Console.WriteLine($"{Name} has health of {Health} / {MaxHealth}. Potion is drunk automatically!");
                    UseConsumables.UseConsumItem(consumitem, this);
                    Inventory.TryRemove(items[i]);
                    healthrestored = true;
                }
            }
        }
        public override void AddItemtoInventory(Item item)
        {
            if (item is EquipItem equipitem)
            {
                TryEquip(equipitem);
                return;
            }
            else if (!Inventory.TryAdd(item))
            {
                Console.WriteLine($"Inventory of {Name} is full. {item.Name} was lost...");
            }
            else
            {
                Console.WriteLine($"You added some {item.Name} to your Inventory.");
            }
        }
        public override void AddItemfromUnittoInventory(Unit unit)
        {
            if (unit?.Inventory?.Items == null) return;
            var loot = unit.Inventory.Items
                .OrderBy(i=> i is EquipItem ? 0 : 1)
                .ToList();
            foreach (var item in loot)
                {
                    AddItemtoInventory(item);
                    unit.Inventory.TryRemove(item);
                }
        }
        public bool TryEquip(EquipItem newitem, bool autoreplace = false)
        {
            var slot = newitem switch
            {
                RangeWeapon _ => EquipSlot.RangeWeapon,
                Weapon _ => EquipSlot.Weapon,
                Helmet _ => EquipSlot.Helmet,
                Armor _ => EquipSlot.Armor,
                _ => throw new InvalidOperationException("Unknown equipment, you don't know how to wear it...")
            };
            if (_equipment.TryGetValue(slot, out var olditem) && olditem != null)
            {
                if (!autoreplace)
                {
                    Console.WriteLine($"You've found a new {newitem.Name} ! " +
                        $"\nWould you like to replace your {olditem.Name} with it ? " +
                        $"\nIf you would, please press letter Y. If not - press any else letter...");
                    if (Console.ReadKey().Key != ConsoleKey.Y) return false;
                    Console.WriteLine();
                }
                if (Inventory.TryAdd(olditem))
                    Console.WriteLine($"Your {olditem.Name} was replaced by {newitem.Name} and hidden into your bag!");
                else
                {
                    Console.WriteLine($"Your {olditem.Name} was replaced by {newitem.Name} !");
                    Console.WriteLine("You dropped your old equipment on the floor in this room as you have no more space in your bag..." +
                        "\nAnd the old equipment was broken off... You lost it...");
                }
            }
            else
            {
                _equipment[slot] = newitem;
                Console.WriteLine($"You equipped {newitem.Name} !");
            }
            return true;
        }
        protected override uint CalculateSufferedDamage(uint damage)
        {
            Armor armor = null;
            Helmet helmet = null;
            bool hasarmor = _equipment.TryGetValue(EquipSlot.Armor, out var item) && (armor = item as Armor) != null;
            bool hashelmet = _equipment.TryGetValue(EquipSlot.Helmet, out var helmitem) && (helmet = helmitem as Helmet) != null;
            if (hasarmor && hashelmet)
            {
                uint defense = (armor.Defense + helmet.Defense);
                damage -= (uint)(damage * (defense / 100f));
                return damage;
            }
            if (hasarmor)
            {
                damage -= (uint)(damage * (armor.Defense / 100f));
                return damage;
            }
            if (hashelmet)
                damage -= (uint)(damage * (helmet.Defense / 100f));
            return damage;
        }
        public Weapon? EquippedWeapon => _equipment.TryGetValue(EquipSlot.Weapon, out var weapitem) ? weapitem as Weapon : null;
        public Armor? EquippedArmor => _equipment.TryGetValue(EquipSlot.Armor, out var armitem) ? armitem as Armor : null;
        public RangeWeapon? EquippedRangeWeapon => _equipment.TryGetValue(EquipSlot.RangeWeapon, out var rweapitem) ? rweapitem as RangeWeapon : null;
        public Helmet? EquippedHelmet => _equipment.TryGetValue(EquipSlot.Helmet, out var helmitem) ? helmitem as Helmet : null;
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Name);
            builder.AppendLine($"Health: {Health} / {MaxHealth}");
            if (_equipment.TryGetValue(EquipSlot.RangeWeapon, out var rweapitem) && rweapitem is RangeWeapon rangeweapon)
            {
                builder.AppendLine($"Armed with: {rangeweapon.Name} (damage {rangeweapon.Damage}, dur {rangeweapon.Durability} / {rangeweapon.MaxDurability})");
            }
            if (_equipment.TryGetValue(EquipSlot.Weapon, out var weapitem) && weapitem is Weapon weapon)
            {
                builder.AppendLine($"Armed with: {weapon.Name} (damage {weapon.Damage}, dur {weapon.Durability} / {weapon.MaxDurability})");
            }
            else builder.AppendLine("Not Armed");
            if (_equipment.TryGetValue(EquipSlot.Armor, out var armitem) && armitem is Armor armor)
            {
                builder.AppendLine($"Armored with: {armor.Name} (defense {armor.Defense} %, dur {armor.Durability} / {armor.MaxDurability})");
            }
            else builder.AppendLine("Not Armored");
            if (_equipment.TryGetValue(EquipSlot.Helmet, out var helmitem) && helmitem is Helmet helmet)
            {
                builder.AppendLine($"Covered with: {helmet.Name} (defense {helmet.Defense} %, dur {helmet.Durability} / {helmet.MaxDurability})");
            }
            else builder.AppendLine("Not Covered");
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
