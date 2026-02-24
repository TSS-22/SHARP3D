using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Linq;
using System.Text;
using System.Text.Json;


namespace SHARP3D.Parameter
{
    public static class C3dParameterHelper
    {
        // TODO:
        // Create that at run time? is it possible ?
        // Have a function that gather the json values and return them as a list to be used by other function
        // Create a function to assign the parameter to their supported type
        // Put the ParametersFromFile from the ParameterGroup struct in there.

        // Keeping this a ref if needed
        //private static readonly Dictionary<string, SupportedParameter> Map = new Dictionary<string, SupportedParameter>
        //{
        //    // REQUIRED PARAMETERS
        //    // POINT
        //    // Required
        //    { "POINT:USED", SupportedParameterType.Force },
        //    { "POINT:SCALE", SupportedParameterType.Force },


        //};
        private static SupportedParameter[] ArraySupportedParameter;
        private static string RequiredParameterPath;
        private static string AdditionalParameterPath;
        private static string ApplicationParameterPath;
        private static string UserParameterPath;
        private static readonly object _lock = new object();

        static C3dParameterHelper()
        {
            lock (_lock)
            {
                Reset();
            }
        }

        public static void Reset()
        {
            // Set json file path 
            RequiredParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "RequiredParameters.json");
            AdditionalParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "AdditionalParameters.json");
            ApplicationParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ApplicationParameters.json");
            UserParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "UserParameters.json");

            // Load Required
            SupportedParameter[] requiredParameters = LoadJson(RequiredParameterPath);
            
            // Load Additional
            SupportedParameter[] additionalParameters = LoadJson(AdditionalParameterPath);

            // Load Application
            SupportedParameter[] applicationParameters = LoadJson(ApplicationParameterPath);

            // Load user
            SupportedParameter[] userParameters = LoadJson(UserParameterPath);

            // Aggregate and refresh ListSupportedParameter
            ArraySupportedParameter = requiredParameters.Concat(additionalParameters).Concat(applicationParameters).Concat(userParameters).ToArray();
        }

        public static SupportedParameter[] LoadJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File path is invalid: {filePath}");
            }
            string json = File.ReadAllText(filePath);

            JsonSupportedParameter[] tempJsonSupportedParameter = JsonSerializer.Deserialize<JsonSupportedParameter[]>(json)
                ?? throw new InvalidOperationException($"Failed to deserialize JSON from file: {filePath}");
            return tempJsonSupportedParameter.Select(temp =>
                new SupportedParameter(
                    temp.Group,
                    temp.Name,
                    temp.ParameterType,
                    temp.GeneralDescription,
                    temp.DimensionDescription
                )
            ).ToArray();

        }

        public static SupportedParameter[] GetAllSupportedParameter() 
        {
            return ArraySupportedParameter;
        }

        public static SupportedParameter FromString(string groupName, string parameterName)
        {
            foreach (SupportedParameter parameter in ArraySupportedParameter)
            {
                if ((parameter.Group == groupName.ToUpper()) && (parameter.Name == parameterName.ToUpper())) 
                {
                    return parameter;    
                }
            }
            throw new InvalidOperationException($"The parameter {groupName.ToUpper()}:{parameterName.ToUpper()} is not supported yet.");
        }

        // TODO: https://en.wikipedia.org/wiki/UTF-8#Error_handling
        public static List<C3dParameterGroup> ParametersFromFileStreams(FileStream c3dStream, ProcessorType processorFile, int pointerDataSection, int pointerParameterSection = 512)
        {
            int[] scalarDimension = { 1 };
            c3dStream.Seek(pointerParameterSection + 4, SeekOrigin.Begin);
            List<C3dParameterGroup> groups = new List<C3dParameterGroup> { };
            List<C3dParameter> parameters = new List<C3dParameter> { };

            // Get all the Groups and Parameters
            int pointerToNextStruct = 0;
            // TODO: WTF AM I SUPPOSE TO WITH THE PARAMETER AS THEY DON'T FOLLOW THE RULE FOR ENDING.
            // WARNING: THE POINTER TO NEXT STRUCT IS NOT 0X00 0X00 FOR THE LAST PARAMETER BLOCK......
            do
            {
                if (c3dStream.Position == ((pointerDataSection - 1) * 512))
                {
                    break;
                }
                // Not ready for the loop this typeBlock statement
                sbyte nameLength = (sbyte)c3dStream.ReadByte();
                if (nameLength == 0)
                {
                    c3dStream.Seek(-1, SeekOrigin.Current);
                    break;
                }
                int id = (sbyte)c3dStream.ReadByte();
                byte[] nameBuffer = new byte[Math.Abs((int)nameLength)];
                c3dStream.ReadExactly(nameBuffer);
                string name = Encoding.ASCII.GetString(nameBuffer).TrimEnd('\0');
                byte[] pointerBuffer = new byte[2];
                c3dStream.ReadExactly(pointerBuffer);
                // TODO: Check the cast, I might need to do a C3dBytesConvertorToUInt() function.
                pointerToNextStruct = C3dBytesConvertor.ToUInt(pointerBuffer, processorFile);

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
                        descriptionBuffer = new byte[pointerToNextStruct - 3];
                        c3dStream.ReadExactly(descriptionBuffer);
                        actualDescriptionLength = descriptionBuffer.Length;
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
                            Locked = nameLength < 0 ? true : false,
                            Parameters = new List<C3dParameter> { }
                        }
                        );
                }
                else // Parameter
                {
                    // Parameter fields variables
                    int numberOfDimensions;
                    ParameterDataType dataLength;
                    int[]? dimensions = null;
                    byte[] dimensionsBuffer;
                    byte[] dataBuffer;
                    Array data;

                    dataLength = (ParameterDataType)(sbyte)c3dStream.ReadByte(); // TODO: Test this black magic lol
                    numberOfDimensions = c3dStream.ReadByte();

                    if (numberOfDimensions > 0) // Non scalar
                    {
                        dimensions = new int[numberOfDimensions];
                        dimensionsBuffer = new byte[numberOfDimensions];
                        c3dStream.ReadExactly(dimensionsBuffer);
                        for (int i = 0; i < dimensionsBuffer.Length; i++)
                        {
                            dimensions[i] = (int)dimensionsBuffer[i]; // Cast byte to int
                        }

                        dataBuffer = new byte[Math.Abs((int)dataLength) * dimensions.Aggregate((acc, val) => acc * val)];
                        switch (dataLength)
                        {
                            case ParameterDataType.CHAR:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<char>(
                                    dataBuffer,
                                    dimensions,
                                    dataLength,
                                    processorFile
                                    ); // Does that work? crazy
                                break;
                            case ParameterDataType.BYTE:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<byte>(
                                    dataBuffer,
                                    dimensions,
                                    dataLength,
                                    processorFile
                                    );
                                break;
                            case ParameterDataType.INT16:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<int>(
                                    dataBuffer,
                                    dimensions,
                                    dataLength,
                                    processorFile
                                    );
                                break;
                            case ParameterDataType.FLOAT32:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<float>(
                                    dataBuffer,
                                    dimensions,
                                    dataLength,
                                    processorFile
                                    );
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
                            descriptionBuffer = new byte[pointerToNextStruct - 3 - 1 - 1 - dimensionsBuffer.Length - dataBuffer.Length]; // Black magic yeah! Joke: we take out the byte already read, because the pointer to next struct, start at the first byte of the pointer.
                            c3dStream.ReadExactly(descriptionBuffer);
                            actualDescriptionLength = descriptionBuffer.Length;
                        }

                        description = Encoding.UTF8.GetString(descriptionBuffer).TrimEnd('\0');
                    }
                    else // Scalar
                    {
                        dataBuffer = new byte[Math.Abs((int)dataLength)];
                        switch (dataLength)
                        {
                            case ParameterDataType.CHAR:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<char>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataLength,
                                    processorFile
                                    ); // Does that work? crazy
                                break;
                            case ParameterDataType.BYTE:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<byte>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataLength,
                                    processorFile
                                    );
                                break;
                            case ParameterDataType.INT16:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<int>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataLength,
                                    processorFile
                                    );
                                break;
                            case ParameterDataType.FLOAT32:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<float>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataLength,
                                    processorFile
                                    );
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
                            descriptionBuffer = new byte[pointerToNextStruct - 3 - 1 - 1 - Math.Abs((int)dataLength)];
                            c3dStream.ReadExactly(descriptionBuffer);
                            actualDescriptionLength = descriptionBuffer.Length;
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
            groups = groups.Select(group =>
            {
                group.Parameters = parameters.Where(parameter => parameter.Id == group.Id * -1).ToList();
                return group;
            }).ToList();

            return groups.ToList();
        }
    }
}
