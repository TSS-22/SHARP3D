using SHARP3D.Data.Enum;
using SHARP3D.Data.Parameter.ParameterData;

namespace SHARP3D.data.Parameter
{
    public struct C3dParameter
    {
        int NameLength;
        int Id;
        string Name;
        int PointerNextParameterStruct;
        DataLength DataType;
        int NbOfDimensions;
        int[]? Dimensions; // Do int[1] for scalar so it is consistent qith multidimensionnal.
        ParameterData Data; // HOW THE FUCK AM I SUPPOSED TO REPRESENT THIS
        int DescriptionLength;
        string Description;
        bool locked;
    }
}
