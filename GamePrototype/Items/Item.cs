namespace GamePrototype.Items
{
    public abstract class Item
    {
        public string Name { get; }
        protected Item(string name) 
        {
            Name = name;
            Amount = 1;
        }
        public abstract bool Stackable { get; }
        public virtual uint Amount { get; protected set; }
        public bool TryStack(Item item)
        {
            if (!Stackable)
            {
                return false;
            }
            else
            {
                Amount++;
                return true;
            }
        }
    }
}
