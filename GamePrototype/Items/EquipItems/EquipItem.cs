using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems
{
    public abstract class EquipItem : Item
    {
        private uint _durability;
        private uint _maxdurability;
        public uint Durability { get => _durability; protected set => _durability = value; }
        public void Repair(uint delta) => _durability += _durability + delta > _maxdurability
        ? _maxdurability
        : _durability + delta;
        public abstract EquipSlot Slot { get; }
        protected EquipItem(uint maxdurability, string name) : base(name) => _maxdurability = maxdurability;
        public override bool Stackable => false;
    }
}
