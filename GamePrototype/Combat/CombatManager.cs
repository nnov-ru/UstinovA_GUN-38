using GamePrototype.Items.EquipItems;
using GamePrototype.Units;
namespace GamePrototype.Combat
{
    public sealed class CombatManager
    {
        private readonly Random _random = new();

        public Unit StartCombat(Unit player, Unit enemy) => PlayCombatRoutine(player, enemy);
        private Unit PlayCombatRoutine(Unit player, Unit enemy)
        {
            Console.WriteLine(GetCombatString());
            while (player.Health > 0 && enemy.Health > 0)
            {
                string input = Console.ReadLine();
                if (!int.TryParse(input, out int choice) || choice < 1 || choice > 3)
                {
                    Console.WriteLine("Invalid choice! Re-read rules please and asap, Goblin doesn't wait for you !");
                    Console.WriteLine(GetCombatString());
                    continue;
                }
                if (Enum.TryParse<RockPaperScissors>(input, out var rockpaperscissors))
                {
                    HandleCombatInput(player, enemy, rockpaperscissors);
                }
                else
                {
                    Console.WriteLine(GetCombatString());
                }
            }
            if (player.Health > 0 && enemy.Health == 0)
            {
                return player;
            }
            else if (player.Health == 0 && enemy.Health > 0)
            {
                return enemy;
            }
            return null;
        }
        private string GetCombatString() => $"Type {(int)RockPaperScissors.Rock} for {RockPaperScissors.Rock}" +
            $" or {(int)RockPaperScissors.Paper} for {RockPaperScissors.Paper}" +
            $" or {(int)RockPaperScissors.Scissors} for {RockPaperScissors.Scissors}";
        private void HandleCombatInput(Unit player, Unit enemy, RockPaperScissors rockpaperscissors)
        {
            var enemyinput = (RockPaperScissors) _random.Next(1, 4);
            Console.WriteLine($"{player.Name}'s choice ({rockpaperscissors}) against {enemy.Name}'s one ({enemyinput})");
            switch (rockpaperscissors)
            {
                //player hits
                case RockPaperScissors.Rock when enemyinput == RockPaperScissors.Scissors :
                    SufferDamage(player, enemy);
                    break;
                case RockPaperScissors.Paper when enemyinput == RockPaperScissors.Rock :
                    SufferDamage(player, enemy);
                    break;
                case RockPaperScissors.Scissors when enemyinput == RockPaperScissors.Paper :
                    SufferDamage(player, enemy);
                    break;
                //enemy hits
                case RockPaperScissors.Scissors when enemyinput == RockPaperScissors.Rock :
                    SufferDamage(enemy, player);
                    break;
                case RockPaperScissors.Rock when enemyinput == RockPaperScissors.Paper :
                    SufferDamage(enemy, player);
                    break;
                case RockPaperScissors.Paper when enemyinput == RockPaperScissors.Scissors :
                    SufferDamage(enemy, player);
                    break;
                default:
                    Console.WriteLine("Nobody received a hit. Type 1 or 2 or 3 again");
                    break;
            }
        }
        private void SufferDamage(Unit attacker, Unit attacked)
        {
            attacked.SufferDamage(attacker.DoUnitDamage());
            if (attacker is Player aer && aer.EquippedWeapon is Weapon weapon)
            {
                weapon.ReduceDurability(1);
            }
            if (attacker is Player aer1 && aer1.EquippedRangeWeapon is RangeWeapon rangeweapon)
            {
                rangeweapon.ReduceDurability(1);
            }
            if (attacked is Player aed && aed.EquippedArmor is Armor armor)
            {
                armor.ReduceDurability(1);
            }
            if (attacked is Player aed1 && aed1.EquippedHelmet is Helmet helmet)
            {
                helmet.ReduceDurability(1);
            }
            Console.WriteLine($"{attacker.Name} hits {attacked.Name}. " +
                                $"{attacked.Name}'s health: {attacked.Health} / {attacked.MaxHealth}");
            if (attacked.Health == 0)
            {
                Console.WriteLine($"{attacked.Name} is dead !");
            }
        }

    }
}
