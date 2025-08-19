using GamePrototype.Items.ConsumItems;
using GamePrototype.Utils;
namespace GamePrototype.Items.EquipItems
{
    public sealed class RangeWeapon : EquipItem
    {
        public RangeWeapon(uint damage, uint maxdurability, string name) : base(maxdurability, name)
        {
            Damage = damage;
            Durability = (int)MaxDurability;
        }
        public uint Damage { get; }
        public override EquipSlot Slot => EquipSlot.RangeWeapon;
        protected override uint GetDamaged(uint delta)
        {
            return delta;
        }
    }
}