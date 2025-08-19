using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems
{
    public abstract class EquipItem : Item
    {
        private int _durability;
        private uint _maxdurability;
        public uint MaxDurability => _maxdurability;
        public int Durability { get => _durability; protected set => _durability = Math.Max(value, 0); }
        public abstract EquipSlot Slot { get; }
        protected EquipItem(uint maxdurability, string name) : base(name) => _maxdurability = maxdurability;
        public override bool Stackable => false;
        public void ReduceDurability(uint delta) => Durability -= (int)GetDamaged(delta);
        protected abstract uint GetDamaged(uint damage);
        public void Repair(uint delta) => Durability = Math.Min((int)MaxDurability, Durability + (int)delta);
    }
}
