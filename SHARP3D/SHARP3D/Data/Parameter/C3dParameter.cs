using SHARP3D.Data.Enum;
using SHARP3D.Data.Parameter.ParameterData;

namespace SHARP3D.data.Parameter
{
    public struct C3dParameter
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public int PointerNextParameterStruct;
        public DataLength DataType;
        public int NbOfDimensions;
        public int[]? Dimensions; // Do int[1] for scalar so it is consistent qith multidimensionnal.
        public ParameterData Data;
        public int DescriptionLength;
        public string Description;
        public bool Locked;
    

    public static C3dParameter FromBinaries(
            sbyte nameLength,
            sbyte id,
            string name,
            int pointerNextParameterStruct,
            int descriptionLength,
            string description,
            bool locked,
            DataLength dataType,
            int nbOfDimensions,
            int[]? dimensions,
            byte[] dataBytes
            )
        {
            ParameterData data = new ParameterData(); //TODO: implement this;

            return new C3dParameter
            {
                NameLength = nameLength,
                Id = id,
                Name = name,
                PointerNextParameterStruct = pointerNextParameterStruct,
                DataType = dataType,
                NbOfDimensions = nbOfDimensions,
                Dimensions = dimensions, // Do int[1] for scalar so it is consistent qith multidimensionnal.
                Data = data,
                DescriptionLength = descriptionLength,
                Description = description,
                Locked = locked,
            };
        }
    }
}
