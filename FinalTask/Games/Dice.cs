using FinalTask.Exceptions;
namespace FinalTask.Games
{
    public struct Dice
    {
        private int _min;
        private int _max;
        private static readonly Random _random = new Random();
        public int Number => _random.Next(_min, _max + 1);
        public Dice(int min, int max)
        {
            if (min < 1)
                throw new WrongDiceNumberException(1, int.MaxValue);
            if (max > int.MaxValue)
                throw new WrongDiceNumberException(1, int.MaxValue);
            if (min > max)
                throw new WrongDiceNumberException(1, min);
            _min = min;
            _max = max;
        }
    }
}
