using GamePrototype.Items.EquipItems;
namespace GamePrototype.Items.ConsumItems
{
    public sealed class Grindstone : ConsumItem
    {
        public uint DurabilityRestore => 4;
        public override bool Stackable => false;
        public Grindstone(string name) : base(name)
        {
        }
    }
}
