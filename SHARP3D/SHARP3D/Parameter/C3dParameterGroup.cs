using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Text;

namespace SHARP3D.Parameter
{
    public struct C3dParameterGroup
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public uint PointerNextParameterStruct; // From the pointer position to the next data structure
        public int DescriptionLength;
        public long ActualDescriptionLength; // Because of UTF-8. TODO: Check if it is necessary to make the distinction.
        public string Description;
        public bool Locked; // For later and the correctors
        public List<C3dParameter> Parameters;

        // TODO: https://en.wikipedia.org/wiki/UTF-8#Error_handling

    }
}
