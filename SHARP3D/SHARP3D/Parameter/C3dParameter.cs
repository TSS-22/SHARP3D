using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter
{
    public struct C3dParameter
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public uint PointerNextParameterStruct;
        public ParameterDataType DataType;
        public int NbOfDimensions;
        public int[]? Dimensions; // Do int[1] for scalar so it is consistent qith multidimensionnal.
        public Array Data; // Doing this because it gives me shit with the abstract class
        public int DescriptionLength;
        public string Description;
        public bool Locked;

    }
}

