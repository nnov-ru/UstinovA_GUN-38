namespace GamePrototype.Items.ConsumItems
{
    public sealed class HealthPotion : ConsumItem
    {
        public uint HealthRestore => 7;
        public override bool Stackable => false;
        public HealthPotion(string name) : base(name) { }
    }
}
