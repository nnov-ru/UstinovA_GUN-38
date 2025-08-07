using Structures;

namespace Classes
{
    public class Weapon
    {
        public string Name { get; }
        public float Durability { get; } = 1f;
        public Interval Damage { get; private set; }
        
        public Weapon(string name)
        {
            Name = name;
        }
        public Weapon(string name, Interval damage) : this(name)
        {
            Damage = damage;
        }
        public int GetDamage()
        {
            return Damage.Get();
        }
    }
}