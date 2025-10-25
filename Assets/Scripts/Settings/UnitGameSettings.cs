using System;
using UnityEditor;
using UnityEngine;

namespace Unity3D
{
    [CreateAssetMenu(menuName = "Settings/UnitGameSettings", fileName = "New UnitGameSetiings", order = 1)]
    public class UnitGameSettings : ScriptableObject
    {
        [field:SerializeField]
        public UnityStats Stats {  get; private set; }
        [field:SerializeField]
        public UnitMobility Mobility { get; private set; }
    }
    [Serializable]
    public struct UnitStats
    {
        public int Health;
        public int Damage;
    }
    [Serializable]
    public struct UnitMobility
    {
        public int QueenMove;
        public int NonStopAttack;
    }

}
