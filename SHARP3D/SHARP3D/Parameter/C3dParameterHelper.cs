using SHARP3D.Parameter.Data;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Linq;
using System.Text;
using System.Text.Json;


namespace SHARP3D.Parameter
{
    /// <summary>
    /// Provides helper methods for loading, parsing, and managing C3D file parameters and parameter groups.
    /// </summary>
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
        /// <summary>
        /// An array of all supported parameters loaded from JSON files.
        /// </summary>
        private static SupportedParameter[] ArraySupportedParameter;

        /// <summary>
        /// The file path for required parameters JSON.
        /// </summary>
        private static string RequiredParameterPath;

        /// <summary>
        /// The file path for additional parameters JSON.
        /// </summary>
        private static string AdditionalParameterPath;

        /// <summary>
        /// The file path for application-specific parameters JSON.
        /// </summary>
        private static string ApplicationParameterPath;

        /// <summary>
        /// The file path for user-defined parameters JSON.
        /// </summary>
        private static string UserParameterPath;

        /// <summary>
        /// An object used for locking to ensure thread safety.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Initializes the <see cref="C3dParameterHelper"/> class by loading all supported parameters.
        /// </summary>
        static C3dParameterHelper()
        {
            lock (_lock)
            {
                Reset();
            }
        }

        /// <summary>
        /// Resets and reloads all supported parameters from their respective JSON files.
        /// </summary>
        public static void Reset()
        {
            // Set json file path 
            RequiredParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "RequiredParameters.json");
            AdditionalParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "AdditionalParameters.json");
            ApplicationParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ApplicationParameters.json");
            UserParameterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "UserDefinedParameters.json");

            // Load Required
            SupportedParameter[]? requiredParameters = LoadJson(RequiredParameterPath);
            
            // Load Additional
            SupportedParameter[]? additionalParameters = LoadJson(AdditionalParameterPath);

            // Load Application
            SupportedParameter[]? applicationParameters = LoadJson(ApplicationParameterPath);

            // Load user
            SupportedParameter[]? userParameters = LoadJson(UserParameterPath);

            // Aggregate and refresh ListSupportedParameter
            ArraySupportedParameter =
                (requiredParameters ?? Enumerable.Empty<SupportedParameter>())
                .Concat(additionalParameters ?? Enumerable.Empty<SupportedParameter>())
                .Concat(applicationParameters ?? Enumerable.Empty<SupportedParameter>())
                .Concat(userParameters ?? Enumerable.Empty<SupportedParameter>())
                .ToArray();
        }

        /// <summary>
        /// Loads supported parameters from a JSON file.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>An array of <see cref="SupportedParameter"/> loaded from the JSON file.</returns>
        /// <exception cref="ArgumentException">Thrown if the file path is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the JSON deserialization fails.</exception>
        public static SupportedParameter[]? LoadJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File path is invalid: {filePath}");
            }
            string json = File.ReadAllText(filePath);

            Dictionary<string, JsonSupportedParameter> tempDict = JsonSerializer.Deserialize<Dictionary<string, JsonSupportedParameter>>(json)
                ?? throw new InvalidOperationException($"Failed to deserialize JSON from file: {filePath}");
            SupportedParameter[] supportedParameter = tempDict.Select(temp =>
            {
                string[] keyParts = temp.Key.Split('-');
                return new SupportedParameter(
                    keyParts[0],
                    keyParts[1],
                    temp.Value.ParameterType,
                    temp.Value.GeneralDescription,
                    temp.Value.DimensionDescription
                );
            }).ToArray();
            return supportedParameter;
        }

        /// <summary>
        /// Gets all supported parameters.
        /// </summary>
        /// <returns>An array of all <see cref="SupportedParameter"/>.</returns>
        public static SupportedParameter[] GetAllSupportedParameter() 
        {
            return ArraySupportedParameter;
        }

        /// <summary>
        /// Retrieves a <see cref="SupportedParameter"/> by its group and parameter name.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The <see cref="SupportedParameter"/> matching the group and parameter name.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the parameter is not supported.</exception>
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

        /// <summary>
        /// Parses C3D parameter groups and parameters from a file stream.
        /// </summary>
        /// <param name="c3dStream">The file stream of the C3D file.</param>
        /// <param name="processorFile">The processor type used in the C3D file.</param>
        /// <param name="pointerParameterSection">The pointer to the parameter section in the C3D file.</param>
        /// <param name="pointerDataSection">The pointer to the data section in the C3D file.</param>
        /// <returns>A list of <see cref="C3dParameterGroup"/> parsed from the file stream.</returns>
        /// <remarks>
        /// <para>
        /// Empty groups (which have no parameters linked to them) are discarded. This is implemented due to Vicon potentially adding twice PROCESSING group, once empty and once with the actual parameters.
        /// </para>
        /// TODO: Handle UTF-8 error cases as per https://en.wikipedia.org/wiki/UTF-8#Error_handling
        /// </remarks>
        // TODO: https://en.wikipedia.org/wiki/UTF-8#Error_handling
        public static List<C3dParameterGroup> ParametersFromFileStreams(FileStream c3dStream, ProcessorType processorFile, int pointerParameterSection, int pointerDataSection)
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
                if (c3dStream.Position == pointerDataSection)
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
                    DataType dataLength;
                    int[]? dimensions = null;
                    byte[] dimensionsBuffer;
                    byte[] dataBuffer;
                    Array data;

                    dataLength = (DataType)(sbyte)c3dStream.ReadByte(); // TODO: Test this black magic lol
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
                        // Sometimes the C3D parameter is saved having multiple dimensions but in fact some of them are going to be zero. Which cause problems.
                        // Because they dimmed that it was a good way to make optionnal parameters that depends on other parameters... That you don't know if you will see before this parameter coming....
                        // Big brain time les gars! #ClapClapClap
                        if ((dimensions.Aggregate((acc, val) => acc * val)) == 0)
                        {
                            dataBuffer = new byte[0];
                            switch (dataLength)
                            {
                                case DataType.CHAR:
                                    data = Array.CreateInstance(typeof(char), 0);
                                    break;
                                case DataType.BYTE:
                                    data = Array.CreateInstance(typeof(byte), 0);
                                    break;
                                case DataType.INT16:
                                    data = Array.CreateInstance(typeof(int), 0);
                                    break;
                                case DataType.FLOAT32:
                                    data = Array.CreateInstance(typeof(float), 0);
                                    break;
                                default:
                                    throw new Exception("Invalid data type length");
                            }
                        }
                        else
                        {
                            dataBuffer = new byte[Math.Abs((int)dataLength) * dimensions.Aggregate((acc, val) => acc * val)];
                            switch (dataLength)
                            {
                                case DataType.CHAR:
                                    c3dStream.ReadExactly(dataBuffer);
                                    data = Fortran.VectorToMatrix<char>(
                                        dataBuffer,
                                        dimensions,
                                        dataLength,
                                        processorFile
                                        ); // Does that work? crazy
                                    break;
                                case DataType.BYTE:
                                    c3dStream.ReadExactly(dataBuffer);
                                    data = Fortran.VectorToMatrix<byte>(
                                        dataBuffer,
                                        dimensions,
                                        dataLength,
                                        processorFile
                                        );
                                    break;
                                case DataType.INT16:
                                    c3dStream.ReadExactly(dataBuffer);
                                    data = Fortran.VectorToMatrix<int>(
                                        dataBuffer,
                                        dimensions,
                                        dataLength,
                                        processorFile
                                        );
                                    break;
                                case DataType.FLOAT32:
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
                            case DataType.CHAR:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<char>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataLength,
                                    processorFile
                                    ); // Does that work? crazy
                                break;
                            case DataType.BYTE:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<byte>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataLength,
                                    processorFile
                                    );
                                break;
                            case DataType.INT16:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<int>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataLength,
                                    processorFile
                                    );
                                break;
                            case DataType.FLOAT32:
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
                            Locked = nameLength < 0 ? true : false,
                            Supported = SupportedParameter.UnkownParameter()
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

            // Discard empty groups
            groups = groups.Where(g => g.Parameters.Count > 0).ToList();


            // Discard duplicate parameter, because Vicon like to put some duplicate
            foreach (var group in groups)
            {
                var uniqueParameters = group.Parameters
                    .GroupBy(p => p.Name)
                    .Select(g => g.First())
                    .ToList();

                group.Parameters.Clear();
                foreach (var param in uniqueParameters)
                {
                    group.Parameters.Add(param);
                }
            }

            // TODO: Change this as it is useless at the moment and detrimental for use of the end user .dll
            for (int i = 0; i < groups.Count; i++)
            {
                for(int j=0; j < groups[i].Parameters.Count; j++)
                {
                    try
                    {
                        C3dParameter tempParameter = groups[i].Parameters[j];
                        tempParameter.Supported = C3dParameterHelper.FromString(
                            groups[i].Name, groups[i].Parameters[j].Name
                            );
                        groups[i].Parameters[j] = tempParameter;
                    }
                    catch(InvalidOperationException ex)
                    { 
                        // Do nothing as the C3dParameter.Supported field is initialized as unkown by default.
                    }
                }    
            }
            // Create and return the list of group/parameter and their index. Return that as a tuple
            return groups.ToList();
        }
    }


}
