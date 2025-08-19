using GamePrototype.Items.EquipItems;
using GamePrototype.Items;
using GamePrototype.Utils;

namespace GamePrototype.Units
{
    public sealed class Goblin : Unit
    {
        public enum Slot { Weapon }
        private readonly Dictionary<Slot, EquipItem> _goblinequipment = new();
        public Goblin(string name, int health, uint maxhealth, uint basedamage) : base(name, health, maxhealth, basedamage)
        {
        }

        public override uint DoUnitDamage()
        {
            if (_goblinequipment.TryGetValue(Slot.Weapon, out var weapon) && weapon is Weapon w)
            {
                return BaseDamage + w.Damage;
            }
            return BaseDamage;
        }
        public override void HandleCombatCompleted() 
        { 
            Health = (int)MaxHealth; 
            Console.WriteLine($"Goblin won. His health restored as it was before you came in to him."+
                $"{Health} / {MaxHealth}");
        }
        protected override uint CalculateSufferedDamage(uint damage) => damage;
    }
}
