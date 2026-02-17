using SHARP3D.Parameter.ParameterDataType;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SHARP3D.Parameter
{
    ///<summary>
    ///This structure regroup the C3D parameters from the file. They determine the endian format used. For some very logical reasons they need to be processed before the header could be processed.
    ///</summary>
    public struct C3dParameterBlock {

        public List<C3dParameterGroup> Groups;

        public static C3dParameterBlock FromFileStream(FileStream c3dStream, ProcessorType processorMakerType, int pointerParameterSection = 512)
        {
            c3dStream.Seek(pointerParameterSection, SeekOrigin.Begin);
            List<C3dParameterGroup> groups = new List<C3dParameterGroup> { };
            List<C3dParameter> parameters = new List<C3dParameter> { };

            // Get all the Groups and Parameters
            uint pointerToNextStruct = 0;
            do
            {
                // Not ready for the loop this typeBlock statement
                sbyte nameLength = (sbyte)c3dStream.ReadByte();
                int id = (sbyte)c3dStream.ReadByte();
                byte[] nameBuffer = new byte[nameLength];
                c3dStream.ReadExactly(nameBuffer);
                string name = Encoding.ASCII.GetString(nameBuffer).TrimEnd('\0');
                byte[] pointerBuffer = new byte[2];
                c3dStream.ReadExactly(pointerBuffer);
                // TODO: Check the cast, I might need to do a C3dBytesConvertorToUInt() function.
                pointerToNextStruct = (uint)C3dBytesConvertor.ToInt(pointerBuffer, processorMakerType);

                int descriptionLength;
                long actualDescriptionLength;
                byte[] descriptionBuffer;
                string description;

                if (id < 0) // Group
                {
                    // TODO: Check that this is the actual way of doing it (because UTF8 encoding allowing between 1 to 4 bytes per character
                    // I am not sure that if I just read the descriptionLength I will get the right result. So I compute using the position
                    // after I checked the description length byte and then compute the actual description length, using the position
                    // and the pointer to the next struct (attention: case when pointer = 0....) to compute the actual UT8 compatible description length
                    // TODO: CASE WHEN POINTER = 0. Read until byte == 0x00
                    descriptionLength = c3dStream.ReadByte();
                    if (pointerToNextStruct == 0)
                    {
                        // Read until byte == 0x00
                        List<byte> descriptionBytes = new List<byte> { };
                        int nextByte;
                        while ((nextByte = c3dStream.ReadByte()) != 0x00 && nextByte != -1)
                        {
                            descriptionBytes.Add((byte)nextByte);
                        }
                        actualDescriptionLength = descriptionBytes.Count;
                        descriptionBuffer = descriptionBytes.ToArray();
                    }
                    else
                    {
                        actualDescriptionLength = pointerToNextStruct - c3dStream.Position;
                        descriptionBuffer = new byte[Math.Abs(actualDescriptionLength)];
                        c3dStream.ReadExactly(descriptionBuffer);
                    }
                    
                    description = Encoding.UTF8.GetString(descriptionBuffer).TrimEnd('\0');
                    // Group
                    groups.Add(
                        new C3dParameterGroup
                        {
                            NameLength = nameLength,
                            Id = id,
                            Name = name,
                            PointerNextParameterStruct = pointerToNextStruct,
                            DescriptionLength = descriptionLength,
                            ActualDescriptionLength = actualDescriptionLength,
                            Description = description,
                            Locked = nameLength < 0 ? true : false
                        }
                        );
                }
                else // Parameter
                {
                    // Parameter fields variables
                    int numberOfDimensions;
                    DataLength dataLength;
                    int[]? dimensions = null;
                    byte[] dimensionsBuffer;
                    byte[] dataBuffer;
                    ParameterData data;

                    dataLength = (DataLength)(sbyte)c3dStream.ReadByte(); // TODO: Test this black magic lol
                    numberOfDimensions = c3dStream.ReadByte();
                    
                    if (numberOfDimensions > 0) // Non scalar
                    {
                        dimensionsBuffer = new byte[numberOfDimensions];
                        c3dStream.ReadExactly(dimensionsBuffer);
                        for (int i = 0; i < dimensionsBuffer.Length; i++)
                        {
                            dimensions[i] = (int)dimensionsBuffer[i]; // Cast byte to int
                        }

                        dataBuffer = new byte[Math.Abs((int)dataLength)* dimensions.Aggregate((acc, val) => acc * val)];
                        switch (dataLength)
                        {
                            case DataLength.CHAR:
                                c3dStream.ReadExactly(dataBuffer);
                                data = new MultiCharParameterData(dataBuffer, dimensions, processorMakerType); // Does that work? crazy
                                break;
                            case DataLength.BYTE:
                                c3dStream.ReadExactly(dataBuffer);
                                data = new MultiByteParameterData(dataBuffer, dimensions, processorMakerType);
                                break;
                            case DataLength.INT16:
                                c3dStream.ReadExactly(dataBuffer);
                                data = new MultiIntParameterData(dataBuffer, dimensions, processorMakerType);
                                break;
                            case DataLength.FLOAT32:
                                c3dStream.ReadExactly(dataBuffer);
                                data = new MultiFloatParameterData(dataBuffer, dimensions, processorMakerType);
                                break;
                            default:
                                throw new Exception("Invalid data type length");
                        }
                        descriptionLength = c3dStream.ReadByte();
                        if (pointerToNextStruct == 0)
                        {
                            // Read until byte == 0x00
                            List<byte> descriptionBytes = new List<byte> { };
                            int nextByte;
                            while ((nextByte = c3dStream.ReadByte()) != 0x00 && nextByte != -1)
                            {
                                descriptionBytes.Add((byte)nextByte);
                            }
                            actualDescriptionLength = descriptionBytes.Count;
                            descriptionBuffer = descriptionBytes.ToArray();
                        }
                        else
                        {
                            actualDescriptionLength = pointerToNextStruct - c3dStream.Position;
                            descriptionBuffer = new byte[Math.Abs(actualDescriptionLength)];
                            c3dStream.ReadExactly(descriptionBuffer);
                        }

                        description = Encoding.UTF8.GetString(descriptionBuffer).TrimEnd('\0');
                    }
                    else // Scalar
                    {
                        dataBuffer = new byte[Math.Abs((int)dataLength)];
                        switch (dataLength)
                        {
                            case DataLength.CHAR:
                            dataBuffer = new byte[] { (byte)c3dStream.ReadByte() };
                                data = new CharParameterData(dataBuffer); // Does that work? crazy
                                break;
                            case DataLength.BYTE:
                                dataBuffer = new byte[] { (byte)c3dStream.ReadByte() };
                                data = new ByteParameterData(dataBuffer);
                                break;
                            case DataLength.INT16:
                                c3dStream.ReadExactly(dataBuffer);
                                data = new IntParameterData(dataBuffer, null, processorMakerType);
                                break;
                            case DataLength.FLOAT32:
                                c3dStream.ReadExactly(dataBuffer);
                                data = new FloatParameterData(dataBuffer, null, processorMakerType);
                                break;
                            default:
                                throw new Exception("Invalid data type length");
                        }
                        descriptionLength = c3dStream.ReadByte();
                        if (pointerToNextStruct == 0)
                        {
                            // Read until byte == 0x00
                            List<byte> descriptionBytes = new List<byte> { };
                            int nextByte;
                            while ((nextByte = c3dStream.ReadByte()) != 0x00 && nextByte != -1)
                            {
                                descriptionBytes.Add((byte)nextByte);
                            }
                            actualDescriptionLength = descriptionBytes.Count;
                            descriptionBuffer = descriptionBytes.ToArray();
                        }
                        else
                        {
                            actualDescriptionLength = pointerToNextStruct - c3dStream.Position;
                            descriptionBuffer = new byte[Math.Abs(actualDescriptionLength)];
                            c3dStream.ReadExactly(descriptionBuffer);
                        }

                        description = Encoding.UTF8.GetString(descriptionBuffer).TrimEnd('\0');
                    }
                    parameters.Add(
                        new C3dParameter
                        {
                            NameLength = nameLength,
                            Id = id,
                            Name = name,
                            PointerNextParameterStruct = pointerToNextStruct,
                            DataType = dataLength,
                            NbOfDimensions = numberOfDimensions,
                            Dimensions = dimensions,
                            Data = data,
                            DescriptionLength = descriptionLength,
                            Description = description,
                            Locked = nameLength < 0 ? true : false
                        }
                    );
                }
            } while (pointerToNextStruct != 0);

            // Associate each parameter to its respective group

            return new C3dParameterBlock
            {
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