namespace Classes
{
    public class Unit
    {
        private float _health;
        public string Name { get; }
        public float Health => _health;
        public int Damage { get; }
        public float Armor { get; }
        
        public Unit() : this(name:"Unknown Unit")
        {
        }
        public Unit(string name, int damage = 5, float armor = 0.6f )
        {
            Name = name;
            Damage = damage;
            Armor = armor;
        }
        public float GetRealHealth()
        {
            return Health * (1f + Armor);
        }
        public bool SetDamage(float value)
        {
            _health = Health - value * Armor;
            return (Health <= 0f);
        }
    }
}
