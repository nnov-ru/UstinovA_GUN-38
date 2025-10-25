using System.Collections.Generic;
namespace Unity3D
    {
    public interface IGameplayCommand
    {
        IEnumerable<Cell> Variants { get; }
        void Interact(Cell cell);
    }
}
