using SHARP3D.Utils.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace SHARP3D.Parameter
{
    public struct C3dParameterGroup
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public int PointerNextParameterStruct; // From the pointer position to the next data structure
        public int DescriptionLength;
        public string Description;
        public bool Locked; // For later and the correctors

        // TODO: https://en.wikipedia.org/wiki/UTF-8#Error_handling
        public static C3dParameterGroup FromBinaries(byte[] binaries, ProcessorType processorType)
        {
            sbyte nameLength = (sbyte)binaries[0];
            int absNameLengthMath = Math.Abs(nameLength);
            int descriptionLength = binaries[2 + absNameLengthMath + 2];

            return new C3dParameterGroup
            {
                NameLength = nameLength,
                Id = (sbyte)binaries[1],
                Name = Encoding.ASCII.GetString(binaries.Skip(2).Take(absNameLengthMath).ToArray()).TrimEnd('\0'),
                PointerNextParameterStruct = C3dBytesConvertor.ToInt(binaries.Skip(2 + absNameLengthMath).Take(2).ToArray(), processorType),
                DescriptionLength = descriptionLength,
                Description = Encoding.UTF8.GetString(binaries.Skip(2 + absNameLengthMath + 2 + 1).Take(descriptionLength).ToArray()).TrimEnd('\0'),
                Locked = nameLength < 0
            };
        }

        public static C3dParameterGroup FromBinaries(
            sbyte nameLength,
            sbyte id,
            string name,
            int pointerNextParameterStruct,
            int descriptionLength,
            string description,
            bool locked
            )
        {

            return new C3dParameterGroup
            {
                NameLength = nameLength,
                Id = id,
                Name = name,
                PointerNextParameterStruct = pointerNextParameterStruct,
                DescriptionLength = descriptionLength,
                Description = description,
                Locked = locked
            };
        }

        public static List<C3dParameterGroup> GroupsFromFileStream(FileStream c3dStream, ProcessorType processorMakerType, int pointerParameterSection)
        {
            List<C3dParameterGroup> groups = new List<C3dParameterGroup> { };
            List<C3dParameter> parameters = new List<C3dParameter> { };

            c3dStream.Seek(pointerParameterSection + 4, SeekOrigin.Begin);
            
            // Get all the Groups and Parameters
            int pointerToNextStruct = 0;
            do
            {
                // Not ready for the loop this typeBlock statement
                sbyte nameLength = (sbyte)c3dStream.ReadByte();
                sbyte id = (sbyte)c3dStream.ReadByte();
                
                byte[] bufferName = new byte[nameLength];
                c3dStream.ReadExactly(bufferName, 0, nameLength);

                byte[] bufferPointerNextParameterStruct = new byte[2];
                c3dStream.ReadExactly(bufferPointerNextParameterStruct, 0, 2);

                // Group
                if (id < 0)
                {
                    
                    int descriptionLength = c3dStream.ReadByte();

                    byte[] bufferDescription = new byte[descriptionLength];
                    c3dStream.ReadExactly(bufferDescription, 0, descriptionLength);

                    groups.Add(FromBinaries(
                        nameLength,
                        id,
                        name: Encoding.ASCII.GetString(bufferName).TrimEnd('\0'),
                        pointerNextParameterStruct: C3dBytesConvertor.ToInt(bufferPointerNextParameterStruct, processorMakerType),
                        descriptionLength,
                        description: Encoding.UTF8.GetString(bufferDescription).TrimEnd('\0'),
                        locked: nameLength < 0
                            )
                        );
                    pointerToNextStruct = groups.Last().PointerNextParameterStruct;
                }
                // Parameter
                else
                {
                    DataLength dataType = c3dStream.ReadByte() switch
                    {
                        1 => DataLength.BYTE,
                        2 => DataLength.INT16,
                        4 => DataLength.INT32,
                        -1 => DataLength.CHAR,
                        _ => throw new Exception($"Unknown data type for parameter {Encoding.ASCII.GetString(bufferName).TrimEnd('\0')}")
                    };
                    int nbOfDimensions = c3dStream.ReadByte();
                    int[]? dimensions = null;
                    byte[] parameterDataByte;
                    // Multi-dimensional parameter data
                    if (nbOfDimensions > 0)
                    {
                        dimensions = new int[nbOfDimensions];
                        for (int i = 0; i < nbOfDimensions; i++)
                        { 
                            dimensions[i] = c3dStream.ReadByte();
                        }
                        parameterDataByte = new byte[Math.Abs((int)dataType) * dimensions.Sum()];
                    }
                    // Scalar parameter data
                    else
                    {
                        parameterDataByte = new byte[Math.Abs((int)dataType)];
                    }

                    c3dStream.ReadExactly(parameterDataByte, 0, parameterDataByte.Length);

                    int descriptionLength = c3dStream.ReadByte();

                    byte[] bufferDescription = new byte[descriptionLength];
                    c3dStream.ReadExactly(bufferDescription, 0, descriptionLength);

                    // Parameter
                    parameters.Add(C3dParameter.FromBinaries(
                        nameLength,
                        id,
                        name: Encoding.ASCII.GetString(bufferName).TrimEnd('\0'),
                        pointerNextParameterStruct: C3dBytesConvertor.ToInt(bufferPointerNextParameterStruct, processorMakerType),
                        descriptionLength,
                        description: Encoding.UTF8.GetString(bufferDescription).TrimEnd('\0'),
                        locked: nameLength < 0,
                        dataType: dataType,
                        nbOfDimensions: nbOfDimensions,
                        dimensions: dimensions,
                        dataBytes: parameterDataByte,
                        processor: processorMakerType
                            )
                        );
                    pointerToNextStruct = parameters.Last().PointerNextParameterStruct;
                }
            } while (pointerToNextStruct != 0);

            // Associate each parameter to its respective group
            return groups;
        }
    }
}
