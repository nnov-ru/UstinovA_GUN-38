using UnityEngine;

namespace Unity3D
{
    [CreateAssetMenu(fileName = "New CellPaletteSettings", menuName = "Settings/CellPaletteSettings", order = 52)]
    public class CellPaletteSettings : ScriptableObject
    {
        [field: SerializeField, Tooltip("Under Selected Unit")]
        public Material SelectedCell { get; private set; }
        [field: SerializeField, Tooltip("Eligible for moving to")]
        public Material MoveToCell { get; private set; }
        [field: SerializeField, Tooltip("Eligible for attacking against")]
        public Material AttackCell { get; private set; }
        //[field: SerializeField, Tooltip("Just moved from and to")]
        //public Material MovedToAndFrom { get; private set; }
    }
}