
namespace SHARP3D.data.Parameter
{
    public struct C3dParameterGroup
    {
        int NameLength;
        int Id;
        string Name;
        int PointerNextParameterStruct; // From the pointer position to the next data structure
        int DescriptionLength;
        string Description;
        bool locked; // For later and the correctors
        
        public static C3dParameterGroup FromBinaries(byte[] binaries)
        {
            return new C3dParameterGroup
            {

            }
        }
    
    }
}
