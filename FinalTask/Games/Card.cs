namespace FinalTask.Games
{
    public struct Card
    {
        public CardTypes Type { get; }
        public CardNames Name { get; }
        public Card(CardTypes type, CardNames name)
        {
            Type = type;
            Name = name;
        }
        public override string ToString() => $"{Name} of {Type}";
    }
}
