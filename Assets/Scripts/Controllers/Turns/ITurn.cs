namespace Unity3D
{
    public interface ITurn
    {
        Team Current { get; }
        void Next();
    }
}
