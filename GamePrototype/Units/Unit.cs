using GamePrototype.Items;

namespace GamePrototype.Units
{
    public abstract class Unit
    {
        private const int InventorySize = 3;
        private int _health;
        private uint _maxhealth;
        protected uint BaseDamage;
        protected Inventory Inventory;

        public string Name { get; private set; }
        public int Health
        {
            get => Math.Max(_health, 0); 
            protected set => _health = value; 
        }
        public uint MaxHealth => _maxhealth;
        protected Unit(string name, int health, uint maxhealth, uint basedamage)
        {
            Name = name;
            _health = health;
            BaseDamage = basedamage;
            _maxhealth = maxhealth;
            Inventory = new Inventory(InventorySize);
        }
        public void ApplyDamage(uint damage)
        {
            _health -= (int)CalculateAppliedDamage(damage);
            DamageReceivedHandler();
        }
        protected abstract uint CalculateAppliedDamage(uint damage);
        protected virtual void DamageReceivedHandler() { }
        public abstract uint GetUnitDamage();
        public abstract void HandleCombatCompleted();
        public virtual void AddItemtoInventory(Item item)
        {
            if (!Inventory.TryAdd(item))
            {
                Console.WriteLine($"Inventory of {Name} is full");
            }
        }
        public void AddItemfromUnittoInventory(Unit unit) 
        {
            for (int i = 0; i < unit.Inventory.Items.Count; i++)
            { 
                if (!Inventory.TryAdd(unit.Inventory.Items[i]))
                {
                    return;
                }
            }
        }
    }
}
