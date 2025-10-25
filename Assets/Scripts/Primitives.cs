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
    public enum GameStatus
    {
        Error = 0,
        Locked = 1,
        Unlocked = 2,
        Selecting = 3,
        Motion = 4,
        Attacking = 5,
        Confirmed = 6
    }
    public enum GameEvent
    {
        Empty = 0,
        Select = 1,
        Cancel = 2,
        Confirm = 3
    }
    //temp
    public enum IsQueen
    {
        No = 0,
        Yes = 1,
    }
    }