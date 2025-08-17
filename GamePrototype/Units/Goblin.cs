using GamePrototype.Items.EquipItems;
using GamePrototype.Items;

namespace GamePrototype.Units
{
    public sealed class Goblin : Unit
    {
        public Goblin(string name, int health, uint maxhealth, uint basedamage) : base(name, health, maxhealth, basedamage)
        {
        }

        public override uint GetUnitDamage() => BaseDamage;
        public override void HandleCombatCompleted() => Health = (int)MaxHealth;
        protected override uint CalculateAppliedDamage(uint damage) => damage;
        
    }
}
