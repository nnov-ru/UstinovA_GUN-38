namespace FinalTask.Profile
{
    public class PlayerProfile
    {
        public string Name { get; set; }
        public int Bank { get; set; }
        public const int MaxBank = 1000000;

        public PlayerProfile()
        {
            Bank = 1000;
        }
    }
}
