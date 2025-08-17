namespace GamePrototype.Items.ConsumItems
{
    public sealed class Grindstone : ConsumItem
    {
        public override bool Stackable => false;

        public Grindstone(string name) : base(name)
        {
        }    
    }
}
