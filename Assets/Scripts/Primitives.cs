namespace Unity3D
{
    public enum NeighbourType
    {
        ForwardRight,
        Forward,
        ForwardLeft,
        Right,
        Left,
        BackRight,
        Back,
        BackLeft
    }
    public enum Team
    {
        Player1,
        Player2
    }
    public struct NeighbourCell
    {
        public NeighbourType Type;
        public Cell Cell;
        public NeighbourCell(NeighbourType type, Cell cell)
        {
            Type = type; 
            Cell = cell; 
        }
    }
}