using Structures;
namespace Classes
{
    public class Unit
    {
        private float _health;
        public string Name { get; }
        public float Health => _health;
        public Interval Damage { get; }
        public float Armor { get; }

        public Unit() : this("Unknown Unit", new Interval(0, 5), 0.6f)
        {
        }
        public Unit(string name) : this(name, new Interval(0, 5), 0.6f)
        {
        }
        public Unit(string name, Interval damage) : this(name, damage, 0.6f)
        { 
        }
        public Unit(string name, Interval damage, float armor )
        {
            Name = name;
            Damage = damage;
            Armor = armor;
        }
        public Unit(string name, int mindamage, int maxdamage) :
            this(name, new Interval(mindamage,maxdamage), 0.6f)
        {
        }
        public float GetRealHealth()
        {
            return Health * (1f + Armor);
        }
        public bool SetDamage(float value)
        {
            _health = Health - value * Armor;
            return Health <= 0f;
        }
    }
}
