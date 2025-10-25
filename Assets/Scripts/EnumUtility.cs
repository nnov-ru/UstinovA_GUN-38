using System;

namespace Unity3D
{
    public static class EnumUtility
    {
        public static NeighbourType[] All
        {  
            get
            {
                return (NeighbourType[])Enum.GetValues(typeof(NeighbourType));
            }
        }
    }
}
