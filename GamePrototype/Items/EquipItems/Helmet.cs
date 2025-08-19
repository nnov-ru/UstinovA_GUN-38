using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems
{
    public sealed class Helmet : EquipItem
    {
        public Helmet(uint defense, uint maxdurability, string name) : base(maxdurability, name)
        {
            Defense = defense;
            Durability = (int)maxdurability;
        }
        public uint Defense { get; }
        public override EquipSlot Slot => EquipSlot.Helmet;
        protected override uint GetDamaged(uint delta)
        {
            return delta;
        }
    }
}