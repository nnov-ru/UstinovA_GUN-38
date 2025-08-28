using FinalTask.CasinoMechanics;
using FinalTask.Exceptions;
using FinalTask.Profile;

namespace FinalTask.Games
{
    public class DiceGame : CasinoGameBase
    {
        private readonly int _quantityOfDice;
        private readonly int _maxNumber;
        private readonly int _minNumber;
        private List<Dice> _dice;
        public DiceGame(int quantityOfDice, int minNumber, int maxNumber)
        {
            _quantityOfDice = quantityOfDice;
            _maxNumber = maxNumber;
            _minNumber = minNumber;
            FactoryMethod();
        }
        protected override void FactoryMethod()
        {
            _dice = new List<Dice>();
            for (int i = 0; i < _quantityOfDice; i++)
            {
                _dice.Add(new Dice(_minNumber, _maxNumber));
            }
        }
        private int RollDice()
        {
            int total = 0;
            Console.WriteLine($"\n{_quantityOfDice} dice are thrown:");
            for (int i = 0; i < _dice.Count; i++)
            {
                int result = _dice[i].Number;
                Console.WriteLine($"* Dice {i + 1} = {result}");
                total += result;
            }
            return total;
        }
        public override void PlayGame(int bet)
        {
            int playerRoll = RollDice();
            Console.WriteLine($"You gained {playerRoll} points!");
            int computerRoll = RollDice();
            Console.WriteLine($"Computer gained {computerRoll} points!");
            if (playerRoll > computerRoll)
            {
                Console.WriteLine("You successfully win!");
                OnWinInvoke(bet);
            }
            else if (playerRoll < computerRoll)
            {
                Console.WriteLine("You sadly lose...");
                OnLooseInvoke(bet);
            }
            else
            {
                Console.WriteLine("It's a Draw...");
                OnDrawInvoke(bet);
            }
        }
    }
}
