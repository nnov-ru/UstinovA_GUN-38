using FinalTask.Exceptions;
using FinalTask.Games;
using FinalTask.Profile;
using FinalTask.SaveLoad;

namespace FinalTask.CasinoMechanics
{
    public class Casino : IGame
    {
        private readonly ISaveLoadService<string> _saveLoadService;
        private PlayerProfile _playerProfile;
        private readonly Blackjack21 _blackjack21;
        private readonly DiceGame _diceGame;

        public Casino(ISaveLoadService<string> saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _blackjack21 = new Blackjack21(36);
            try
            {
                _diceGame = new DiceGame(2, 1, 6);
            }
            
            catch (WrongDiceNumberException exc)
            {
                Console.WriteLine($"Dice Numbers Check Error: {exc.Message}");
                Console.WriteLine("Using default dice numbers (from 1 to 6)");
                _diceGame = new DiceGame(2, 1, 6);
            }
            catch (ArgumentException exc) when (exc.ParamName == "quantityOfDice")
            {
                Console.WriteLine($"Dice Quantity Error: {exc.Message}");
                Console.WriteLine("Using default dice quantity (2)");
                _diceGame = new DiceGame(2, 1, 6);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Unexpected error: {exc.Message}");
                _diceGame = new DiceGame(2, 1, 6);
            }
            SubscribeToGameEvents();
        }
        public void StartGame()
        {
            Console.WriteLine("Welcome to the Casino!");
            while (true)
            {
                LoadOrCreateProfile();

                while (_playerProfile.Bank > 0)
                {
                    Console.WriteLine($"\nHello, {_playerProfile.Name}! Your bank contains: {_playerProfile.Bank} units.");
                    Console.WriteLine("Would you please choose a game:");
                    Console.WriteLine("1. Blackjack");
                    Console.WriteLine("2. Dice Game");
                    Console.WriteLine("3. Exit");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            PlayBlackjack();
                            break;
                        case "2":
                            PlayDiceGame();
                            break;
                        case "3":
                            SaveProfile();
                            Console.WriteLine("Good bye!");
                            return;
                        default:
                            Console.WriteLine("Incorrect choice. Please try again!");
                            break;
                    }
                    CheckBankLimits();
                    SaveProfile();
                }
                Console.WriteLine("No money? Kicked!");
                //if (!AskForNewGame)
                //    break;
            }
        }
        private void LoadOrCreateProfile()
        {
            string savedData = _saveLoadService.LoadData("profile");
            if (savedData != null)
            {
                var parts = savedData.Split('|');
                if (parts.Length == 2)
                {
                    _playerProfile = new PlayerProfile 
                    {
                        Name = parts[0],
                        Bank = int.Parse(parts[1])
                    };
                    Console.WriteLine($"Welcome back, {_playerProfile.Name}!");
                    return;
                }
            }
            CreateNewProfile();
        }
        private void CreateNewProfile()
        {
            Console.WriteLine("No profile found. Creating a new profile...");
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            _playerProfile = new PlayerProfile { Name = name };
            Console.WriteLine($"Welcome, {name}!");
        }
        private void SaveProfile()
        {
            string profileData = $"{_playerProfile.Name}|{_playerProfile.Bank}";
            _saveLoadService.SaveData(profileData, "profile");
        }
        //private bool AskForNewGame()
        //{
        //    while (true)
        //    {
        //        Console.WriteLine("\nWould you like to play incognito under another name?");
        //        Console.WriteLine("1. Yes, please create new profile");
        //        Console.WriteLine("2. No, I will never come back in such a place...");
        //        Console.WriteLine("Your option? 1 or 2? Please type: ");
        //        string choice = Console.ReadLine();
        //        switch (choice)
        //        {
        //            case "1":
        //                DeleteProfile();
        //                return true;
        //            case "2":
        //                Console.WriteLine("Become rich and come back to our Casino! Bye!");
        //            default:
        //                Console.WriteLine("Incorrect option, 1 or 2?");
        //        }
        //    }
        //}
        private int GetBet()
        {
            while (true)
            {
                Console.Write($"Enter your bet (from 1 to {_playerProfile.Bank} units): ");
                if (int.TryParse(Console.ReadLine(), out int bet) && bet > 0 && bet <= _playerProfile.Bank)
                {
                    return bet;
                }
                Console.WriteLine("Incorrect bet amount. Please try again");
            }
        }
        private void CheckBankLimits()
        {
            if (_playerProfile.Bank > PlayerProfile.MaxBank)
            {
                int oldBank = _playerProfile.Bank;
                _playerProfile.Bank -= (_playerProfile.Bank / 2);
                Console.WriteLine($"\nYou wasted half of your bank money in casino’s bar!");
                Console.WriteLine($"Bank decreased from {oldBank} to {_playerProfile.Bank} units.");
            }
        }
        private void HandleWin(int bet)
        {
            _playerProfile.Bank += bet + bet;
            Console.WriteLine($"You won {bet} units and got back your {bet} units bet!");
        }
        private void HandleLose(int bet)
        {
            Console.WriteLine($"You lost {bet} units!");
        }
        private void HandleDraw(int bet)
        {
            _playerProfile.Bank += bet;
            Console.WriteLine($"When it's a draw, bets are returned.");
        }
        private void SubscribeToGameEvents()
        {
            _blackjack21.OnWin += HandleWin;
            _blackjack21.OnLose += HandleLose;
            _blackjack21.OnDraw += HandleDraw;

            _diceGame.OnWin += HandleWin;
            _diceGame.OnLose += HandleLose;
            _diceGame.OnDraw += HandleDraw;
        }
        private void PlayBlackjack()
        {
            int bet = GetBet();
            _playerProfile.Bank -= bet;
            _blackjack21.PlayGame(bet);
        }
        private void PlayDiceGame()
        {
            int bet = GetBet();
            _playerProfile.Bank -= bet;
            _diceGame.PlayGame(bet);
        }
    }
}
