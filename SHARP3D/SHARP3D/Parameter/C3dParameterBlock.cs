using SHARP3D.Utils.Enum;
using System.Linq;

namespace SHARP3D.Parameter
{
    ///<summary>
    ///This structure regroup the C3D parameters from the file. They determine the endian format used. For some very logical reasons they need to be processed before the header could be processed.
    ///</summary>
    public struct C3dParameterBlock {

        public List<C3dParameterGroup> Groups = new List<C3dParameterGroup>{ };

        // TODO: Implement method to parse binaries into C3dParameter struct.
        public static C3dParameterBlock FromBinaries(byte[] binaries, ProcessorType processorMakerType)
        {
            int parameterBlockCount = binaries[2];
            List<C3dParameterGroup> groups = new List<C3dParameterGroup> { };
            List<C3dParameter> parameters = new List<C3dParameter> { };

            // Get all the Groups and Parameters
            int index = 0;
            int pointerToNextStruct = 0;
            do {
                // Not ready for the loop this typeBlock statement
                int typeBlock = (sbyte)binaries[4 + 2 + pointerToNextStruct];
                if (typeBlock < 0)
                {
                    // Group
                    groups.Add(C3dParameterGroup.FromBinaries(binaries.Skip(pointerToNextStruct).Take().ToArray(), processorMakerType));
                    pointerToNextStruct = groups.Last().PointerNextParameterStruct;
                }
                else
                {
                    // Parameter
                    parameters.Add(C3dParameter.FromBinaries(binaries.Skip().Take().ToArray(), processorMakerType));
                    pointerToNextStruct = groups.Last().PointerNextParameterStruct;
                }
            } while (pointerToNextStruct != 0);
            

            // Associate each parameter to its respective group

                return new C3dParameterBlock
                {
                    ParameterBlockCount = parameterBlockCount,
                    FileMakerProcessorType = processorMakerType,
                    Groups = groups
                };
        }

        public static C3dParameterBlock FromFileStream(FileStream c3dStream, ProcessorType processorMakerType, int pointerParameterSection)
        {
            c3dStream.Seek(pointerParameterSection, SeekOrigin.Begin);
            List<C3dParameterGroup> groups = new List<C3dParameterGroup> { };
            List<C3dParameter> parameters = new List<C3dParameter> { };

            // Get all the Groups and Parameters
            int index = 0;
            int pointerToNextStruct = 0;
            do
            {
                // Not ready for the loop this typeBlock statement
                int typeBlock = (sbyte)binaries[4 + 2 + pointerToNextStruct];
                if (typeBlock < 0)
                {
                    // Group
                    groups.Add(C3dParameterGroup.FromBinaries(binaries.Skip(pointerToNextStruct).Take().ToArray(), processorMakerType));
                    pointerToNextStruct = groups.Last().PointerNextParameterStruct;
                }
                else
                {
                    // Parameter
                    parameters.Add(C3dParameter.FromBinaries(binaries.Skip().Take().ToArray(), processorMakerType));
                    pointerToNextStruct = groups.Last().PointerNextParameterStruct;
                }
            } while (pointerToNextStruct != 0);


            // Associate each parameter to its respective group

            return new C3dParameterBlock
            {
                ParameterBlockCount = parameterBlockCount,
                FileMakerProcessorType = processorMakerType,
                Groups = groups
            };
        }

        // TODO: Implement method to convert C3dParameter struct into binaries.
        public static byte[] ToBinaries()
        {
            return new byte[0];
        }
    }
}