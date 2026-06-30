using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Text;


namespace SHARP3D.Parameter
{
    /// <summary>
    /// Provides helper methods for loading, parsing, and managing C3D file parameters and parameter groups.
    /// </summary>
    public static class C3dParameterHelper
    {
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
        public static List<C3dFileParameterGroup> ParametersFromFileStreams(FileStream c3dStream, ProcessorType processorFile, int pointerParameterSection, int pointerDataSection)
        {
            int[] scalarDimension = { 1 };
            c3dStream.Seek(pointerParameterSection + 4, SeekOrigin.Begin);
            List<C3dFileParameterGroup> groups = new List<C3dFileParameterGroup> { };
            List<C3dFileParameter> parameters = new List<C3dFileParameter> { };

            // TO CLEAN
            //string filePathParameterSave = @"C:\Users\hfm\Documents\GitHub\SHARP3D\Ressources\parameters_samplingerrorfiles.csv";

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

                // TO CLEAN
                //File.AppendAllText(filePathParameterSave, name + Environment.NewLine);

                byte[] pointerBuffer = new byte[2];
                c3dStream.ReadExactly(pointerBuffer);
                pointerToNextStruct = C3dBytesConvertor.ToUInt(pointerBuffer, processorFile);
                long positionAtPointer = c3dStream.Position - 2;
                long expectedNextParameterPosition = positionAtPointer + pointerToNextStruct;

                int descriptionLength = 0;
                long actualDescriptionLength = 0;
                byte[] descriptionBuffer;
                string description = "";

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
                        new C3dFileParameterGroup
                        {
                            NameLength = nameLength,
                            Id = id,
                            Name = name,
                            PointerNextParameterStruct = pointerToNextStruct,
                            DescriptionLength = descriptionLength,
                            ActualDescriptionLength = actualDescriptionLength,
                            Description = description,
                            Locked = nameLength < 0 ? true : false,
                            Parameters = new List<C3dFileParameter> { }
                        }
                        );
                }
                else // Parameter
                {
                    // Parameter fields variables
                    int numberOfDimensions;
                    DataType dataTypeFile;
                    int[]? dimensions = null;
                    byte[] dimensionsBuffer;
                    byte[] dataBuffer;
                    Array data;

                    dataTypeFile = (DataType)(sbyte)c3dStream.ReadByte(); // TODO: Test this black magic lol
                    numberOfDimensions = c3dStream.ReadByte();

                    // Because the frames parameters shoudl be read as unsigned INT16. Conversion to signed INT32, the C# default is not an issue. It just matter at the binary conversion level.
                    if((
                        (name=="FRAMES")
                        || (name=="LONG_FRAMES")
                        || (name=="ACTUAL_START_FIELD")
                        || (name=="ACTUAL_END_FIELD")
                        ) && dataTypeFile == DataType.INT16)
                    {
                        dataTypeFile = DataType.UINT16;
                    }

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
                            switch (dataTypeFile)
                            {
                                case DataType.CHAR:
                                    data = Array.CreateInstance(typeof(char), 0);
                                    break;
                                case DataType.BYTE:
                                    data = Array.CreateInstance(typeof(byte), 0);
                                    break;
                                case DataType.UINT16:
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
                            dataBuffer = new byte[Math.Abs((int)dataTypeFile) * dimensions.Aggregate((acc, val) => acc * val)];
                            switch (dataTypeFile)
                            {
                                case DataType.CHAR:
                                    c3dStream.ReadExactly(dataBuffer);
                                    data = Fortran.VectorToMatrix<char>(
                                        dataBuffer,
                                        dimensions,
                                        dataTypeFile,
                                        processorFile
                                        ); // Does that work? crazy
                                    break;
                                case DataType.BYTE:
                                    c3dStream.ReadExactly(dataBuffer);
                                    data = Fortran.VectorToMatrix<byte>(
                                        dataBuffer,
                                        dimensions,
                                        dataTypeFile,
                                        processorFile
                                        );
                                    break;
                                case DataType.UINT16:
                                case DataType.INT16:
                                    c3dStream.ReadExactly(dataBuffer);
                                    data = Fortran.VectorToMatrix<int>(
                                        dataBuffer,
                                        dimensions,
                                        dataTypeFile,
                                        processorFile
                                        );
                                    break;
                                case DataType.FLOAT32:
                                    c3dStream.ReadExactly(dataBuffer);
                                    data = Fortran.VectorToMatrix<float>(
                                        dataBuffer,
                                        dimensions,
                                        dataTypeFile,
                                        processorFile
                                        );
                                    break;
                                default:
                                    throw new Exception("Invalid data type length");
                            }
                        }
                        // Some application just don't give a fuck about the way a parameter is supposed to be defined as per table page 39.
                        // So we check if we got to the position that pointerToNextStructure told us the next parameter is...
                        if (c3dStream.Position != expectedNextParameterPosition) 
                        {
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

                        
                    }
                    else // Scalar
                    {
                        dataBuffer = new byte[Math.Abs((int)dataTypeFile)];
                        switch (dataTypeFile)
                        {
                            case DataType.CHAR:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<char>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataTypeFile,
                                    processorFile
                                    ); // Does that work? crazy
                                break;
                            case DataType.BYTE:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<byte>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataTypeFile,
                                    processorFile
                                    );
                                break;
                            case DataType.UINT16:
                            case DataType.INT16:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<int>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataTypeFile,
                                    processorFile
                                    );
                                break;
                            case DataType.FLOAT32:
                                c3dStream.ReadExactly(dataBuffer);
                                data = Fortran.VectorToMatrix<float>(
                                    dataBuffer,
                                    scalarDimension,
                                    dataTypeFile,
                                    processorFile
                                    );
                                break;
                            default:
                                throw new Exception("Invalid data type length");
                        }
                        // Some application just don't give a fuck about the way a parameter is supposed to be defined as per table page 39.
                        // So we check if we got to the position that pointerToNextStructure told us the next parameter is...
                        if (c3dStream.Position != expectedNextParameterPosition)
                        {
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
                                descriptionBuffer = new byte[pointerToNextStruct - 3 - 1 - 1 - Math.Abs((int)dataTypeFile)];
                                c3dStream.ReadExactly(descriptionBuffer);
                                actualDescriptionLength = descriptionBuffer.Length;
                            }

                            description = Encoding.UTF8.GetString(descriptionBuffer).TrimEnd('\0');
                        }
                    }
                    parameters.Add(
                        new C3dFileParameter
                        {
                            NameLength = nameLength,
                            Id = id,
                            Name = name,
                            PointerNextParameterStruct = pointerToNextStruct,
                            DataTypeFile = dataTypeFile,
                            NbOfDimensions = numberOfDimensions,
                            Dimensions = dimensions,
                            Data = data,
                            DescriptionLength = descriptionLength,
                            Description = description,
                            Locked = nameLength < 0 ? true : false,
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

            // Check if Analog exist:
            bool doesAnalogExist = true;
            try
            {
                groups.First(g => g.Name?.Equals("analog", StringComparison.OrdinalIgnoreCase) == true);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Group ANALOG not found. Cannot create ANALOG:FORMAT parameter. Defaulting to \"SIGNED\" behavior.\nException details: " + ex.Message);
                doesAnalogExist = false;
            }

            if (doesAnalogExist)
            {
                // Check for ANALOG:FORMAT
                // If it doesn't exit, create it and give it the value SIGNED. 
                // Will need to update the test
                Array? analogFormatValue = groups
                    .FirstOrDefault(g => g.Name?.Equals("analog", StringComparison.OrdinalIgnoreCase) == true)
                    .Parameters?
                    .FirstOrDefault(p => p.Name?.Equals("format", StringComparison.OrdinalIgnoreCase) == true)
                    .Data ?? null; // Fallback to null if Name is null

                // Create place holder value for SIGNED and UNSIGNED
                Array signedArrayString = Sharp3dConstants.SignedArrayString;
                Array unsignedArrayString = Sharp3dConstants.UnsignedArrayString;

                if (analogFormatValue == null)
                {
                    // Create ANALOG:FORMAT Parameter
                    Array dataAnalogFormat = Array.CreateInstance(typeof(char), "SIGNED".Length);
                    string analogFormatName = "FORMAT";
                    string analogFormatDescription = "Determine if Analog data and parameter are signed or unsigned integers.";

                    groups.First(g => g.Name?.Equals("analog", StringComparison.OrdinalIgnoreCase) == true).Parameters.Add(
                    new C3dFileParameter
                    {
                        NameLength = (sbyte)analogFormatName.Length,
                        Id = Math.Abs(groups.FirstOrDefault(g => g.Name?.Equals("analog", StringComparison.OrdinalIgnoreCase) == true).Id),
                        Name = analogFormatName,
                        PointerNextParameterStruct = 0, // Is that gonna be a problem later on?
                        DataTypeFile = DataType.CHAR,
                        NbOfDimensions = 1,
                        Dimensions = new int[] { "SIGNED".Length },
                        Data = dataAnalogFormat,
                        DescriptionLength = analogFormatDescription.Length,
                        Description = analogFormatDescription,
                        Locked = true
                    });

                    analogFormatValue = signedArrayString;
                }

                // Read the value of ANALOG:FORMAT
                // Act accordingly
                // If signed, do nothing.
                // If unsigned, do stuff
                if (analogFormatValue == signedArrayString)
                {
                    // Do nothing as it is the default behavior.                   
                }
                else if (analogFormatValue == unsignedArrayString)
                {
                    // Convert ALL the analog parameter that are in signed int16 to unsigned int16. Because that's the only type of parameter that can be affected by that.
                    // At the moment that seems to be our best bet, as the FORMAT parameter behavior (loosely) defined in the doc
                    // and tells people that we should make int16 parameter either sigend or unsigned. 
                    // So we make the assumption that user read the doc and understood it the same way as we did, and that there is no need for exception.
                    for (int g = 0; g < groups.Count; g++)
                    {
                        if (groups[g].Name?.Equals("analog", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            for (int i = 0; i < groups[g].Parameters.Count; i++)
                            {
                                if (groups[g].Parameters[i].DataTypeFile == DataType.INT16)
                                {
                                    // Convert the data from signed to unsigned
                                    int[] signedData = (int[])groups[g].Parameters[i].Data;
                                    int[] unsignedData = new int[signedData.Length];
                                    for (int j = 0; j < signedData.Length; j++)
                                    {
                                        unsignedData[j] = signedData[j] & 0xFFFF; // Masking to get the unsigned value
                                    }
                                    // Fix: Copy struct, modify, then assign back
                                    C3dFileParameter tempParameter = groups[g].Parameters[i];
                                    tempParameter.Data = unsignedData;
                                    tempParameter.DataTypeFile = DataType.UINT16;
                                    groups[g].Parameters[i] = tempParameter;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Unrecognized ANALOG:FORMAT value: {analogFormatValue}. Defaulting to \"SIGNED\" behavior.");
                }
            }

            // Create and return the list of group/parameter and their index. Return that as a tuple
            return groups.ToList();
        }
    }


}
