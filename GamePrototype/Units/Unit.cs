using GamePrototype.Items;
using GamePrototype.Items.ConsumItems;

namespace GamePrototype.Units
{
    public abstract class Unit
    {
        private const int InventorySize = 3;
        private int _health;
        private uint _maxhealth;
        protected uint BaseDamage;
        public Inventory Inventory;

        public string Name { get; private set; }
        public int Health
        {
            get => _health;
            protected set => _health = Math.Max(value, 0); 
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
        public void SufferDamage(uint damage)
        {
            Health -= (int)CalculateSufferedDamage(damage);
            DamageSufferedHandler();
        }
        protected abstract uint CalculateSufferedDamage(uint damage);
        protected virtual void DamageSufferedHandler() { }
        public abstract uint DoUnitDamage();
        public abstract void HandleCombatCompleted();
        public virtual void AddItemtoInventory(Item item)
        {
            if (!Inventory.TryAdd(item))
            {
            
            }
        }
        public virtual void AddItemfromUnittoInventory(Unit unit) 
        {
            for (int i = 0; i < unit.Inventory.Items.Count; i++)
            {
                AddItemtoInventory(unit.Inventory.Items[i]);
            }
        }
        public void RestoreHealth(uint delta)
        {
            Health = Math.Min((int)MaxHealth, Health + (int)delta);
        }
    }
}
