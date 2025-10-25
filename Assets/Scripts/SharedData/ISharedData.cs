namespace Unity3D
{
    public interface ISharedData
    {
        bool Locked { get; set; }
        GameEvent Event { get; set; }
        GameStatus Status { get; set; }
        Cell Destination { get; set; }
        Unit Target { get; set; }

        Unit SelectedUnit { get; set; }
    }
}