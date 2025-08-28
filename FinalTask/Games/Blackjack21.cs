using System.Runtime.CompilerServices;
using FinalTask.CasinoMechanics;

namespace FinalTask.Games
{
    public class Blackjack21 : CasinoGameBase
    {
        private readonly int _numberOfCards;
        private Queue<Card> _deck;
        private List<Card> _playerCards;
        private List<Card> _computerCards;
        public Blackjack21(int numberOfCards)
        {
            if (numberOfCards < 4)
                throw new ArgumentException("Number of cards must be minimum 4", nameof(numberOfCards));
            _numberOfCards = numberOfCards;
            _playerCards = new List<Card>();
            _computerCards = new List<Card>();
        }
        protected override void FactoryMethod()
        {
            CreateDeck();
            Shuffle();
        }
        private void Shuffle()
        {
            var random = new Random();
            var cards = _deck.ToList();
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
            _deck = new Queue<Card>(cards);
        }
        public void CreateDeck()
        {
            var cards = new List<Card>();
            var types = Enum.GetValues(typeof(CardTypes)).Cast<CardTypes>().ToArray();
            var values = Enum.GetValues(typeof(CardNames)).Cast<CardNames>().ToArray();

            int cardsNeeded = Math.Min(types.Length * values.Length, _numberOfCards);
            for (int i = 0; i< cardsNeeded; i++)
                {
                    int typeIndex = i % types.Length;
                    int nameIndex = i % values.Length;
                    cards.Add(new Card(types[typeIndex], values[nameIndex]));
                }
            _deck = new Queue<Card>(cards);
        }
        private int Calculator(List<Card> cards)
        {
            int score = 0;
            foreach (var card in cards)
            {
                if (card.Name == CardNames.Ace)
                {
                    score += 11;
                }
                else if (card.Name >= CardNames.Jack)
                {
                    score += 10;
                }
                else
                {
                    score += (int)card.Name;
                }
            }
            return score;
        }            
        private Card TakeCard()
        {
            if (_deck.Count == 0)
            {
                FactoryMethod();
            }
            return _deck.Dequeue();
        }
        private void ShowCards(string player,List<Card> cards, bool showAll = true)
        {
            Console.WriteLine($"\n{player}:");
            for (int i = 0; i < cards.Count; i++)
            {
                if (showAll || i == 0)
                    Console.WriteLine($"* {cards[i]}");
                else
                    Console.WriteLine("Cards hidden");
            }
            if (showAll)
                Console.WriteLine($"Score: {Calculator(cards)}");
        }
        public override void PlayGame(int bet)
        {
            _playerCards.Clear();
            _computerCards.Clear();

            _playerCards.Add(TakeCard());
            _playerCards.Add(TakeCard());
            _computerCards.Add(TakeCard());
            _computerCards.Add(TakeCard());

            int playerScore = Calculator(_playerCards);
            int computerScore = Calculator(_computerCards);
            ShowCards("You have taken", _playerCards);
            ShowCards("Computer has taken", _computerCards, false);

            bool gameFinished = false;
            while (!gameFinished)
            {
                playerScore = Calculator(_playerCards);
                computerScore = Calculator(_computerCards);

                if (playerScore > 21 || computerScore > 21)
                {
                    DetermineWinner(bet, playerScore, computerScore);
                    gameFinished = true;
                }
                else if (playerScore == computerScore && playerScore < 21)
                {
                    Console.WriteLine("\nEqual scores under 21. Both players take one more card !");
                    _playerCards.Add(TakeCard());
                    _computerCards.Add(TakeCard());
                    ShowCards("Your new set", _playerCards);
                    ShowCards("Computer's new set", _computerCards, false);

                }
                else
                {
                    DetermineWinner(bet, playerScore, computerScore);
                    gameFinished = true;
                }
            }
        }
        private void DetermineWinner(int bet, int playerScore, int computerScore)
        {
            ShowCards("Finally you have", _playerCards);
            ShowCards("Finally the computer has", _computerCards);
            
            if ((playerScore <= 21 && computerScore > 21) || 
                (playerScore <= 21 && computerScore < 21 && playerScore > computerScore))
            {
                Console.WriteLine("You successfully win!");
                OnWinInvoke(bet);
            }
            else if ((computerScore <= 21 && playerScore > 21) ||
                    (computerScore <= 21 && playerScore < 21 && playerScore < computerScore))
            {
                Console.WriteLine("Computer wins! You sadly lose...");
                OnLooseInvoke(bet);
            }
            else if ((playerScore == computerScore && computerScore == 21) ||
                    (playerScore > 21 && computerScore > 21))
            {
                Console.WriteLine("It's a Draw...");
                OnDrawInvoke(bet);
            }

        }
    }
}
