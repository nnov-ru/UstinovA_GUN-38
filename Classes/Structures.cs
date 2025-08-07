using Classes;

namespace Structures
{
    public struct Interval
    {
        private static readonly Random _random = new Random();
        public int Min { get; }
        public int Max { get; }
        public int Get()
        {
            return (_random.Next(Min, Max + 1));
        }

        public Interval(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                (minValue, maxValue) = (maxValue, minValue);
                Console.WriteLine("Wrong input! Min value was less than Max value, so they are interchanged now");
            }
            if (minValue < 0)
            {
                minValue = 0;
                Console.WriteLine("Wrong input! Max value has to be greater than or equal to 0. It was corrected to a minimal value of 0.");
            }
            if (maxValue < 0)
            {
                maxValue = 0;
                Console.WriteLine("Wrong input! Min value has to be greater than or equal to 0. It was corrected to a minimal value of 0.");
            }
            if (minValue == maxValue)
            {
                maxValue += 10;
                Console.WriteLine("Wrong input! Min and Max should not be equal. Max was increased by 10 in order to differentiate");
            }
            Min = minValue;
            Max = maxValue;
        }
    }
    public struct Room
    {
        public Unit Unit { get; }
        public Weapon Weapon { get; }
        public Room(Unit unit, Weapon weapon)
        {
            Unit = unit;
            Weapon = weapon;
        }
    }
}
