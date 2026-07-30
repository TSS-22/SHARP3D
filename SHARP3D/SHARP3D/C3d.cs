using SHARP3D.Data.Clean;
using SHARP3D.Header.DataEntity;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Data.Common;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;

        public C3dFileHeaderEvent[] HeaderEvents = new C3dFileHeaderEvent[] { };

        public C3dRequiredParameters Required = new C3dRequiredParameters();

        public C3dParameterSection Parameters = new C3dParameterSection();

        public C3dData Data = new C3dData();

        public C3d() { }

        public C3d(C3dData data, C3dParameterSection? parameters = null, C3dRequiredParameters? required = null)
        {
            Data = data;
            Required = required != null ? required : new C3dRequiredParameters(); 
            Parameters = parameters != null ? parameters : new C3dParameterSection();
        }

        public C3d(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3dFile.FilePath;

            HeaderEvents = c3dFile.Header.Events;

            Required.Point = new C3dParameterPoint
            {
                Rate = c3dFile.Point.Rate,
                Units = c3dFile.Point.Units,
                Frames = c3dFile.Point.Frames,
                MaximumInterpolationGap = c3dFile.Header.MaxFrameIntepolationGap
            };
            Required.Analog = new C3dParameterAnalog{
                GeneralScale = c3dFile.Analog.GeneralScale,
                SamplesPerFrame = c3dFile.Analog.SamplesPerFrame,
            };

            Parameters = new C3dParameterSection(c3dFile.Parameters);

            Data = new C3dData(c3dFile);

            Parameters.DeleteUneededParametersFromFiles();
        }

        public void ChangePointUnit(string newUnit, float factor)
        {
            Required.Point.Units = newUnit;
            foreach(C3dPointTrajectory trajectory in Data.Points)
            {
                for (int i = 0; i < trajectory.Point.GetLength(0); i++)
                {
                    for(int j=0; j< trajectory.Point.GetLength(1); j++)
                    {
                        trajectory.Point[i,j] = trajectory.Point[i,j] * factor;
                    }
                    trajectory.Residual[i] = trajectory.Residual[i] * factor;
                }
            }
            
        }

        public void ChangeAnalogGeneralScale(float generalScale)
        {
            foreach(C3dAnalogChannel channel in Data.Analogs)
            {
                for(int i=0; i < channel.Data.Length; i++)
                {
                    channel.Data[i] = channel.Data[i] / Required.Analog.GeneralScale * generalScale;
                }
            }

            for(int idPlate = 0; idPlate<Data.Forceplates.Length;idPlate++)
            {
                foreach (C3dAnalogChannel channel in Data.Forceplates[idPlate].Channels)
                {
                    for (int i = 0; i < channel.Data.Length; i++)
                    {
                        channel.Data[i] = channel.Data[i] / Required.Analog.GeneralScale * generalScale;
                    }
                }
            }

            Required.Analog.GeneralScale = generalScale;    
        }

        

        // TODO
        public int AddFrame()
        {
            return Data.Points.Length != 0 ? Data.Points[0].Residual.Length : 0;
        }

        // TODO
        public int DeleteFrame(int idFrameToDelete)
        {
            foreach (C3dPointTrajectory trajectory in Data.Points)
            {

            }

            foreach (C3dAnalogChannel channel in Data.Analogs)
            {

            }

            foreach (C3dForceplate forcePlate in Data.Forceplates)
            {

            }

            return Data.Points.Length != 0 ? Data.Points[0].Residual.Length : 0;
        }

        public void Save(string filePath, FileFrameFormat fileFrameFormat = FileFrameFormat.C3D_STANDARD)
        {
            byte[] parameters = ParametersToBinaries();
            byte[] header = HeaderToBinaries(parameters.Length / 512);
            //byte[] data = DataToBinaries();
            //byte[] c3dBinaries = header.Concat(parameters).Concat(data).ToArray();
        }

        public byte[] ParametersToBinaries()
        {
            
            List<byte> parametersBytes = new List<byte>();
            // Introduction to the parameter section
            parametersBytes.Add((byte)0x00);// Unused/Ignored/Reserved
            parametersBytes.Add((byte)0x00);// Unused/Ignored/Reserved
            parametersBytes.Add((byte)0x00);// Length
            parametersBytes.Add(BitConverter.IsLittleEndian? (byte)0x54 : (byte)0x56);// Processor. Not sure if SIG/MIPS are a general big Endian. But that's the closest.

            // Groups and Parameters to binary
            List<C3dParameterGroup> groups = Parameters.GetGroups();

            // For each Groups
            for (int idGroup = 0; idGroup < groups.Count; idGroup++)
            {
                ////////////////////////////////
                // GROUP
                // Name Length
                parametersBytes.Add(groups[idGroup].Locked? 
                    (byte)(-groups[idGroup].Name.Length) : (byte)groups[idGroup].Name.Length
                    );

                // ID
                parametersBytes.Add((byte)(-(idGroup+1)));

                //Name
                parametersBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(groups[idGroup].Name));

                // Need to know the Description byte length to compute the value of 
                // PointerToNext
                // And we need to know the description byte array to compute its length
                // :O)

                // Description
                byte[] groupDescription = System.Text.Encoding.UTF8.GetBytes(groups[idGroup].Description);
                // Description length
                byte groupDescriptionLength = (byte)groupDescription.Length;
                //Pointer to next
                parametersBytes.AddRange(BitConverter.GetBytes((Int16)(2 + 1 + groupDescriptionLength)));
                parametersBytes.Add(groupDescriptionLength);
                parametersBytes.AddRange(groupDescription);

                // Check if groups is one of the constant group that we manage externally
                // if yes, sort that here

                if (groups[idGroup].Name == "ANALOG")
                {
                    ////////////////////////////////////
                    // ANALOG:BITS
                    List<int> bitsValues = new List<int>();
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        bitsValues.Add(channel.Bits);
                    }
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        for(int i = 0; i < forceplate.Channels.Length; i++)
                        {
                            bitsValues.Add(forceplate.Channels[i].Bits);
                        }
                    }
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "BITS",
                        "Describes the analog data sample resolution in bits.",
                        (int)bitsValues.Average(),
                        true
                    ));

                    // "DESCRIPTIONS[0-9]*",

                    ////////////////////////////////////
                    // ANALOG:FORMAT                    
                    parametersBytes.AddRange(ParameterMonoStringToBinary(
                        idGroup,
                        "FORMAT",
                        "Specifies whether the integer Analog Data and associated integer values Parameters are stored as signed or unsigned 16-bit integer.",
                        "SIGNED", // To help be compatible with old shit. Change that in the future
                        true
                        ));

                    //////////////////////////////////////
                    // ANALOG:GEN_SCALE
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "GEN_SCALE",
                        "A universal common analog scaling factor for all analog channels.",
                        Required.Analog.GeneralScale,
                        true
                    ));
                    // "LABELS[0-9]*",
                    ////////////////////////////////
                    // ANALOG:OFFSET[0-9]*
                    int counter = 0;
                    List<int[]> analogOffsetArrays = new List<int[]>();
                    List<int> bufferOffset = new List<int>();
                    foreach(C3dAnalogChannel channel in Data.Analogs)
                    {
                        bufferOffset.Add(channel.Offset);
                        counter++;
                        if (counter >= 255)
                        {
                            analogOffsetArrays.Add(bufferOffset.ToArray());
                            bufferOffset = new List<int>();   
                            counter = 0;
                        }
                    }
                    counter = 0;
                    foreach (int[] analogOffset in analogOffsetArrays)
                    {
                        parametersBytes.AddRange(Parameter1DArrayToBinary(
                            idGroup,
                            $"OFFSET{counter}",
                            "Store array of integer values that are subtracted from each analog measurement before the individual ANALOG:SCALE scaling factors are applied.",
                            analogOffset,
                            false
                            ));
                        counter++;
                    }

                    ///////////////////////////////
                    // ANALOG:RATE
                    List<float> ratesValues = new List<float>();
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        ratesValues.Add(channel.Rate);
                    }
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        for (int i = 0; i < forceplate.Channels.Length; i++)
                        {
                            ratesValues.Add(forceplate.Channels[i].Rate);
                        }
                    }
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "RATE",
                        "Stores the sample rate at which the analog data was collected in samples per second.",
                        ratesValues.Average(),
                        true
                    ));

                    /////////////////////////////////////
                    // ANALOG:SCALE[0-9]*
                    int counterScale = 0;
                    List<float[]> analogScaleArrays = new List<float[]>();
                    List<float> bufferScale = new List<float>();
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        bufferScale.Add(channel.Scale);
                        counterScale++;
                        if (counter >= 255)
                        {
                            analogScaleArrays.Add(bufferScale.ToArray());
                            bufferScale = new List<float>();
                            counter = 0;
                        }
                    }
                    counterScale = 0;
                    foreach (float[] analogScale in analogScaleArrays)
                    {
                        parametersBytes.AddRange(Parameter1DArrayToBinary(
                            idGroup,
                            $"SCALE{counterScale}",
                            "Stores array of floating-point values that are applied together with the ANALOG:GEN_SCALE parameter value to convert the analog data to physical world values.",
                            analogScale,
                            false
                            ));
                        counterScale++;
                    }
                    // "UNITS[0-9]*",
                    /////////////////////////////////////////
                    // ANALOG:USED
                    int analogUsed =0;
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        analogUsed++;
                    }
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        for (int i = 0; i < forceplate.Channels.Length; i++)
                        {
                            analogUsed++;
                        }
                    }
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "USED",
                        "Stores the number of analog channels that are contained within the C3D file.",
                        analogUsed,
                        true
                    ));
                }
                else if (groups[idGroup].Name == "FORCE_PLATFORM")
                {
                    /////////////////////////////////
                    // FORCE_PLATFORM:CAL_MATRIX
                    List<float[,]> calmatValues = new List<float[,]>();
                    foreach(C3dForceplate forceplate in Data.Forceplates)
                    {
                        calmatValues.Add(forceplate.CalibrationMatrix);
                    }
                    parametersBytes.AddRange(Parameter3DArrayToBinary(
                        idGroup,
                        "CAL_MATRIX",
                        "Stores the calibration matrix that enables software applications to correct for cross talk between outputs of the force platform.",
                        calmatValues.ToArray(),
                        false
                        ));
                    /////////////////////////////////////
                    // FORCE_PLATFORM:CORNERS
                    List<float[,]> cornersValues = new List<float[,]>();
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        cornersValues.Add(forceplate.Corners);
                    }
                    parametersBytes.AddRange(Parameter3DArrayToBinary(
                        idGroup,
                        "CORNERS",
                        "Stores the locations of the force platform corners in the reference coordinate system, measured in POINT:UNITS.",
                        cornersValues.ToArray(),
                        false
                        ));
                    //////////////////////////////
                    // FORCE_PLATFORM:CHANNEL
                    int[,] forceplateChannelValues = new int[8,Data.Forceplates.Length]; // We default to 8 values, without testing if TYPE-3 plates are present. it should work and is easier.
                    int idChannel = 0;
                    for(int i=0; i<Data.Forceplates.Length;i++)
                    {
                        for(int j=0; j< Data.Forceplates[i].Channels.Length; j++)
                        {
                            forceplateChannelValues[j, i] = idChannel;
                            idChannel++;
                        }
                    }
                    parametersBytes.AddRange(Parameter2DArrayToBinary(
                        idGroup,
                        "CHANNEL",
                        "Stores which analog channels index contain specific force platform data.",
                        forceplateChannelValues,
                        false
                        ));
                    ////////////////////////////////
                    // FORCE_PLATFORM:ORIGIN
                    float[,] originValues = new float[3,Data.Forceplates.Length];
                    for(int i =0; i< Data.Forceplates.Length; i++)
                    {
                        for(int j =0; j< 3; j++)
                        {
                            originValues[j,i] = Data.Forceplates[i].Origin[j];
                        }
                    }
                    parametersBytes.AddRange(Parameter2DArrayToBinary(
                        idGroup,
                        "ORIGINS",
                        "Stores the locations of the force platform corners in the reference coordinate system, measured in POINT:UNITS.",
                        originValues,
                        false
                        ));
                    ///////////////////////////////////
                    // FORCE_PLATFORM:TYPE
                    List<int> typeForceplateValues = new List<int>();
                    foreach(C3dForceplate forceplate in Data.Forceplates)
                    {
                        typeForceplateValues.Add((int)forceplate.Type);
                    }
                    parametersBytes.AddRange(Parameter1DArrayToBinary(
                        idGroup,
                        "TYPE",
                        "Define the type of force platform output expected from each force platform.",
                        typeForceplateValues.ToArray(),
                        false
                        ));

                    ////////////////////////////////////////
                    // FORCE_PLATFORM:USED
                    int forceplateUsed = 0;
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        forceplateUsed++;
                    }
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "USED",
                        "Stores the number of force plateform that are contained within the C3D file.",
                        forceplateUsed,
                        true
                    ));
                    ///////////////////////////
                    // FORCE_PLATFORM:ZERO
                   int[] zeroValues = new int[2];
                    foreach(C3dForceplate forceplate in Data.Forceplates)
                    {
                        zeroValues[0] = forceplate.Zero.Item1 + 1;
                        zeroValues[2] = forceplate.Zero.Item2 + 1;
                    }
                    parametersBytes.AddRange(Parameter1DArrayToBinary(
                        idGroup,
                        "ZERO",
                        "Specify the range of 3D data frame numbers that may be used to provide a baseline for the force platform measurements.",
                        zeroValues,
                        false
                        ));

                }
                else if (groups[idGroup].Name == "POINT")
                {

                    // "DESCRIPTIONS[0-9]*",
                    ////////////////////////////////
                    // POINT:FRAMES
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "FRAMES",
                        "Stores the number of 3D data frames that are recorded in the C3D file.",
                        (float)Required.Point.Frames,
                        true
                    ));
                    // "LABELS[0-9]*",
                    // "LONG_FRAMES", Don't bother till I have the different save options
                    //////////////////////////////////
                    // POINT:RATE
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "FRAMES",
                        "Stores the 3D sample rate of the data contained within the C3D file in samples per second.",
                        (float)Required.Point.Rate,
                        true
                    ));
                    ///////////////////////////////
                    // POINT:SCALE
                    float maximumPointValue = 0;
                    foreach (C3dPointTrajectory trajectory in Data.Points) 
                    {
                        foreach(float val in trajectory.Point)
                        {
                            if(Math.Abs(val) > maximumPointValue)
                            {
                                maximumPointValue = val;
                            }
                        }
                    }
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "SCALE",
                        "Stores the scaling factor that is applied to convert each of the signed integer 3D point values into the reference coordinate system values recorded by the POINT:UNITS parameter.",
                        -(maximumPointValue/32000),
                        true
                    ));
                    //////////////////////////////
                    // POINT:UNITS
                    parametersBytes.AddRange(ParameterMonoStringToBinary(
                        idGroup,
                        "UNITS",
                        "four-character ASCII parameter that records the physical measurement environment used by the program that created the 3D Point data stored in the C3D file.",
                        Required.Point.Units,
                        true
                        ));
                    /////////////////////////////
                    // POINT:USED
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "USED",
                        "Stores number of 3D marker trajectories recorded within the C3D file.",
                        Data.Points.Length,
                        true
                    ));
                }
                else if (groups[idGroup].Name == "TRIAL")
                {
                    // Don't bother till I have the different save options
                    // "ACTUAL_END_FIELD",
                    // "ACTUAL_START_FIELD",
                }

                // Sort the other Parameters
                foreach (C3dParameter parameter in groups[idGroup].Parameters) 
                {
                    // Name Length
                    parametersBytes.Add(parameter.Locked ?
                    (byte)(-parameter.Name.Length) : (byte)parameter.Name.Length
                    );

                    // ID
                    parametersBytes.Add((byte)(idGroup+1));

                    // Name
                    parametersBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(parameter.Name));

                    // NEEDED FOR POINTER TO NEXT
                    // Same as for the groups, but more complex
                    // We need to know everything before we can compute pointer to next
                    // (no wonder people don't give a fuck and don't compute it lol)

                    // Data type
                    sbyte parameterDataType;
                    Type dataType = parameter.GetDataType();
                    if (
                        dataType == typeof(byte)
                        || dataType == typeof(sbyte)
                        || dataType == typeof(short)
                        || dataType == typeof(ushort)
                        )
                    {
                        parameterDataType = 1;
                    }
                    else if (
                        dataType == typeof(int)
                        || dataType == typeof(uint)
                        )
                    {
                        parameterDataType = 2;
                    }
                    else if (
                        dataType == typeof(float)
                        || dataType == typeof(double)
                        )
                    {
                        parameterDataType = 4;
                    }
                    else if (
                        dataType == typeof(string)
                        || dataType == typeof(char))
                    {
                        parameterDataType = -1;
                    }
                    else
                    {
                        throw new Exception($"Unsupported data type {dataType} for parameter {parameter.Name}");
                    }

                    // Dimensions number
                    byte parameterDimensionsNumber = (byte)parameter.Dimensions.Length;

                    // Dimensions Length
                    byte[] parameterDimensionsLength = new byte[parameterDimensionsNumber];
                    for(int i = 0; i < parameterDimensionsLength.Length; i++)
                    {
                        parameterDimensionsLength[i] = (byte)parameter.Dimensions[i];
                    }

                    // Data
                    List<byte> parameterData = new List<byte>();
                    if (parameterDimensionsNumber == 0)// If scalar
                    {
                        switch (parameterDataType)
                        {
                            case -1: // Char and string
                                parameterData.Add(
                                    (byte)(parameter.Data?.GetValue(0) as char? ?? throw new NullReferenceException(""))
                                );
                                break;
                            case 1: // Byte
                                parameterData.Add(
                                    parameter.Data?.GetValue(0) as byte? ?? throw new NullReferenceException("")
                                );
                                break;
                            case 2: // Int16
                                parameterData.AddRange(
                                    BitConverter.GetBytes(
                                    parameter.Data?.GetValue(0) as Int16? ?? throw new NullReferenceException("")
                                    )
                                );
                                break;
                            case 4: // Float
                                parameterData.AddRange(
                                    BitConverter.GetBytes(
                                    parameter.Data?.GetValue(0) as float? ?? throw new NullReferenceException("")
                                    )
                                );
                                break;
                            default:
                                throw new Exception($"Unsupported data type {dataType} for parameter {parameter.Name}");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < parameter.Data.Length; i++)
                        {
                            // Need to account for the dimensions
                            // Because with just one index and GetValue, 
                            // Then I put the data as a row matrix and not column matrix.
                            // The 80' wants its good idea back...
                            // Let see if the index work like this or not. That's going to be fun if I need to recompute it.
                            switch (parameterDataType)
                            {
                                case -1: // Char and string
                                    parameterData.Add(
                                        (byte)(parameter.Data?.GetValue(i) as char? ?? throw new NullReferenceException(""))
                                    );
                                    break;
                                case 1: // Byte
                                    parameterData.Add(
                                        parameter.Data?.GetValue(i) as byte? ?? throw new NullReferenceException("")
                                    );
                                    break;
                                case 2: // Int16
                                    parameterData.AddRange(
                                        BitConverter.GetBytes(
                                        parameter.Data?.GetValue(i) as Int16? ?? throw new NullReferenceException("")
                                        )
                                    );
                                    break;
                                case 4: // Float
                                    parameterData.AddRange(
                                        BitConverter.GetBytes(
                                        parameter.Data?.GetValue(i) as float? ?? throw new NullReferenceException("")
                                        )
                                    );
                                    break;
                                default:
                                    throw new Exception($"Unsupported data type {dataType} for parameter {parameter.Name}");
                            }
                        }
                    }
                    // Description
                    byte[] parameterDescription = System.Text.Encoding.UTF8.GetBytes(parameter.Description);
                    // Description length
                    byte parameterDescriptionLength = (byte)parameterDescription.Length;

                    // PointerToNext
                    int pointerToNext = 2 + 1 + 1 + parameterDimensionsLength.Length + parameterData.Count + 1 + parameterDescriptionLength;
                    
                    // Adding all the bytes together
                    parametersBytes.AddRange(BitConverter.GetBytes((Int16)pointerToNext));
                    parametersBytes.Add((byte)parameterDataType);
                    parametersBytes.Add(parameterDimensionsNumber);
                    parametersBytes.AddRange(parameterDimensionsLength);
                    parametersBytes.AddRange(parameterData);
                    parametersBytes.Add(parameterDescriptionLength);
                    parametersBytes.AddRange(parameterDescription);
                }

            }

            // Do POINT:DATA_START
            // This is so dumb: this is supposed to be a pointer to the data.
            // But you need to know the parameter block number to know this.
            // But then once you know it... you add another parameter which can fuck up the count...
            // WOW. Big brain time.
            // Do like that retard pointer architecture: precreate and change the value later.
            // IMPORTANT
            // Put the the pointerToNext to zero here, as this is the last parameter

            // IMPORTANT
            // Pads with zero to finish the blocks so it is a multiple of 512 bytes.


            // Put the length of the parameter section in the third byte
            parametersBytes[2] = (byte)(parametersBytes.Count / 512 + 1);

            
            return parametersBytes.ToArray();
        }

        public byte[] ParameterScalarToBinary(
            int idGroup,
            string name,
            string description,
            int value,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 2;
            // Dimensions numbers
            byte dimensionNumber = 0;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + 2 // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(BitConverter.GetBytes(value));
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] ParameterScalarToBinary(
            int idGroup,
            string name,
            string description,
            float value,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 4;
            // Dimensions numbers
            byte dimensionNumber = 0;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + 4 // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(BitConverter.GetBytes(value));
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] ParameterScalarToBinary(
            int idGroup,
            string name,
            string description,
            byte value,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 1;
            // Dimensions numbers
            byte dimensionNumber = 0;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + 1 // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.Add(value);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] ParameterScalarToBinary(
            int idGroup,
            string name,
            string description,
            char value,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType;
            unchecked { dataType = (byte)-1; }
            // Dimensions numbers
            byte dimensionNumber = 0;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                2 // Pointer to next
                + 1 // Data type
                + 1 // dimension number. There is no data length because it is a scalar
                + 1 // Data bytes
                + 1 // Description length
                + (int)descriptionLength // Description bytes
                ));
            parameterBytes.AddRange(pointerToNext);
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.Add((byte)value);// C# char are 16-bit but C3D char are 8-bit
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] ParameterMonoStringToBinary(
            int idGroup,
            string name,
            string description,
            string value,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            byte[] valueByte = System.Text.Encoding.ASCII.GetBytes(value); // C# char are 16-bit but C3D char are 8-bit
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType;
            unchecked { dataType = (byte)-1; }
            // Dimensions numbers
            byte dimensionNumber = 1;
            // Dimension Length
            byte dimensionsLength = (byte)valueByte.Length;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                2 // Pointer to next is an int
                + 1  // Data type
                + 1 // Dimension number
                + 1 // Dimension length
                + valueByte.Length  // Value bytes
                + 1  // Description length
                + (int)descriptionLength) // Description bytes 
                );
            parameterBytes.AddRange(pointerToNext);
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.Add(dimensionsLength);
            parameterBytes.AddRange(valueByte);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] Parameter1DArrayToBinary(
            int idGroup,
            string name,
            string description,
            float[] arrayData,
            bool locked = false
            )
            {
                List<byte> parameterBytes = new List<byte>();
                byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
                byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
                List<byte> dataBytes = new List<byte>();
                foreach (float value in arrayData)
                {
                    dataBytes.AddRange(BitConverter.GetBytes(value));
                }
                // Name Length
                // Locked parameter
                parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
                // ID
                parameterBytes.Add((byte)(idGroup + 1));
                // Name
                parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
                // Data type
                byte dataType = 4;
                // Dimensions numbers
                byte dimensionNumber = 1;
                //Dimension length
                byte dimensionLength = (byte)dataBytes.Count;
                // Description Length
                byte descriptionLength = (byte)descriptionBytes.Length;
                // Pointer to next
                byte[] pointerToNext = BitConverter.GetBytes((
                                2 // Pointer to next
                                + 1 // Data type
                                + 1 // dimension number. There is no data length because it is a scalar
                                + 1 // dimension length
                                + dataBytes.Count // Data bytes
                                + 1 // Description length
                                + (int)descriptionLength // Description bytes
                                )); parameterBytes.AddRange(pointerToNext);

                parameterBytes.Add(dataType);
                parameterBytes.Add(dimensionNumber);
                parameterBytes.Add(dimensionLength);
                parameterBytes.AddRange(dataBytes);
                parameterBytes.Add(descriptionLength);
                parameterBytes.AddRange(descriptionBytes);

                return parameterBytes.ToArray();
            }

        public byte[] Parameter1DArrayToBinary(
            int idGroup,
            string name,
            string description,
            int[] arrayData,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            List<byte> dataBytes = new List<byte>();
            foreach (int value in arrayData)
            {
                dataBytes.AddRange(BitConverter.GetBytes((Int16)value));
            }
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 4;
            // Dimensions numbers
            byte dimensionNumber = 1;
            //Dimension length
            byte dimensionLength = (byte)dataBytes.Count;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + 1 // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);

            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.Add(dimensionLength);
            parameterBytes.AddRange(dataBytes);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] Parameter2DArrayToBinary(
            int idGroup,
            string name,
            string description,
            byte[,] arrayData,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            List<byte> dataBytes = new List<byte>();
            for (int i = 0; i < arrayData.GetLength(0); i++)
            {
                for (int j = 0; j < arrayData.GetLength(1); j++)
                {
                    dataBytes.Add(arrayData[i, j]);
                }
            }
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 4;
            // Dimensions numbers
            byte dimensionNumber = 2;
            //Dimension length
            byte[] dimensionLength = new byte[] { (byte)arrayData.GetLength(1), (byte)arrayData.GetLength(0)};
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);

            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(dimensionLength);
            parameterBytes.AddRange(dataBytes);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] Parameter2DArrayToBinary(
            int idGroup,
            string name,
            string description,
            int[,] arrayData,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            List<byte> dataBytes = new List<byte>();
            for (int i = 0; i < arrayData.GetLength(0); i++)
            {
                for (int j = 0; j < arrayData.GetLength(1); j++)
                {
                    dataBytes.AddRange(BitConverter.GetBytes((Int16)arrayData[i, j]));
                }
            }
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 2;
            // Dimensions numbers
            byte dimensionNumber = 2;
            //Dimension length
            byte[] dimensionLength = new byte[] { (byte)arrayData.GetLength(1), (byte)arrayData.GetLength(0) };
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);

            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(dimensionLength);
            parameterBytes.AddRange(dataBytes);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] Parameter2DArrayToBinary(
            int idGroup,
            string name,
            string description,
            float[,] arrayData, 
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            List<byte> dataBytes = new List<byte>();
            for(int i=0; i < arrayData.GetLength(0); i++)
            {
                for(int j=0; j < arrayData.GetLength(1); j++)
                {
                    dataBytes.AddRange(BitConverter.GetBytes(arrayData[i, j]));
                }
            }
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 4;
            // Dimensions numbers
            byte dimensionNumber = 2;
            //Dimension length
            byte[] dimensionLength = new byte[] { (byte)(arrayData.GetLength(1) * 4), (byte)(arrayData.GetLength(0) * 4) };
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);
            
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(dimensionLength);
            parameterBytes.AddRange(dataBytes);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }


        public byte[] Parameter3DArrayToBinary(
            int idGroup,
            string name,
            string description,
            int[][,] arrayData,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            List<byte> dataBytes = new List<byte>();
            for (int i = 0; i < arrayData.GetLength(0); i++)
            {
                for (int j = 0; j < arrayData[i].GetLength(0); j++)
                {
                    for (int k = 0; k < arrayData[i].GetLength(1); k++)
                    {
                        dataBytes.AddRange(BitConverter.GetBytes((Int16)arrayData[i][j, k]));
                    }
                }
            }
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 2;
            // Dimensions numbers
            byte dimensionNumber = 3;
            //Dimension length
            byte[] dimensionLength = new byte[] { (byte)arrayData[0].GetLength(0), (byte)arrayData[0].GetLength(1), (byte)arrayData.GetLength(0) };
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);

            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(dimensionLength);
            parameterBytes.AddRange(dataBytes);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] Parameter3DArrayToBinary(
            int idGroup,
            string name,
            string description,
            float[][,] arrayData,
            bool locked = false
            )
        {
            List<byte> parameterBytes = new List<byte>();
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(description);
            List<byte> dataBytes = new List<byte>();
            for (int i = 0; i < arrayData.GetLength(0); i++)
            {
                for (int j = 0; j < arrayData[i].GetLength(0); j++)
                {
                    for (int k = 0; k < arrayData[i].GetLength(1); k++)
                    {
                        dataBytes.AddRange(BitConverter.GetBytes(arrayData[i][j, k])); 
                    }
                }
            }
            // Name Length
            // Locked parameter
            parameterBytes.Add((byte)(locked ? (byte)(-nameBytes.Length) : (byte)nameBytes.Length));
            // ID
            parameterBytes.Add((byte)(idGroup + 1));
            // Name
            parameterBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            // Data type
            byte dataType = 4;
            // Dimensions numbers
            byte dimensionNumber = 3;
            //Dimension length
            byte[] dimensionLength = new byte[] { (byte)arrayData[0].GetLength(0), (byte)arrayData[0].GetLength(1), (byte)arrayData.GetLength(0)};
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + (int)descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);

            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(dimensionLength);
            parameterBytes.AddRange(dataBytes);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] DataToBinaries(float scaleFactor)
        {
            List<byte> data = new List<byte>();

            return data.ToArray();
        }

        public byte[] HeaderToBinaries(int blockLengthParameterSection)
        {
            List<byte> header = new List<byte>();
            //   Byte 1: uint8, Byte 2: char Byte 1: Number of 512 - byte blocks to Parameter Section +1.
            header.Add((byte)0x02);
            //    Byte 2: Data storage format flag.
            header.Add((byte)0x50);
            //2   uint16 Number of markers stored in each Data Frame.
            header.AddRange(BitConverter.GetBytes((UInt16)Data.Points.Length));
            //3   uint16 Total number of analog samples per Data Frame.
            header.AddRange(BitConverter.GetBytes((UInt16)(Required.Analog.SamplesPerFrame * Data.Analogs.Length)));
            //4   uint16 First frame number of raw data(not used / misleading).
            header.AddRange(BitConverter.GetBytes((UInt16)1));
            //5   uint16 Last frame number of raw data(not used / misleading).
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Point.Frames));
            //6   uint16 Maximum 3D frame interpolation gap.
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Point.MaximumInterpolationGap));
            //7 - 8     float32 Data Scale factor.
            float maximumValue = 0f;
            (float?[,,] allPointsData, _, _) = Data.GetAllPointsData();
            foreach(float? point in allPointsData)
            {
                if (point.HasValue && Math.Abs(point.Value) > maximumValue)
                {
                    maximumValue = Math.Abs(point.Value);
                }
            }
            header.AddRange(BitConverter.GetBytes(maximumValue / 32000f));
            //9   uint16 Number of 512 - byte blocks to the Data Section + 1.
            header.AddRange(BitConverter.GetBytes((UInt16)(blockLengthParameterSection + 1)));
            //10  uint16 Analog Frames per Data Frame.
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Analog.SamplesPerFrame));
            //11 - 12   float32     3D Point Data acquisition rate in Hertz.
            header.AddRange(BitConverter.GetBytes(Required.Point.Rate * (float)Required.Analog.SamplesPerFrame));
            //13 - 149  — 	Not used.
            for(int i = 0; i < 137; i++)
            {
                header.AddRange(BitConverter.GetBytes((UInt16)0));
            }
            //150     uint16 Indicates support for 2 or 4 - character Header Event labels.
            header.AddRange(BitConverter.GetBytes((UInt16)12345));
            //151     uint16  Number of Header Events(0 - 18).
            header.AddRange(BitConverter.GetBytes((UInt16)HeaderEvents.Length));
            //152 	— 	Not used.
            header.AddRange(BitConverter.GetBytes((UInt16)0));
            //153 - 188     float32     Header Event times in seconds.
            for (int i = 0; i < 18; i++)
            {
                if (i < HeaderEvents.Length)
                {
                    header.AddRange(BitConverter.GetBytes(HeaderEvents[i].EventTime));
                }
                else
                {
                    header.AddRange(BitConverter.GetBytes(0f));
                }
            }
            //189 - 197     uint8   Header Event flag(0x00 = ON, 0x01 = OFF).
            for (int i = 0; i < 18; i++)
            {
                if (i < HeaderEvents.Length)
                {
                    header.Add((byte)HeaderEvents[i].DisplayFlag);
                }
                else
                {
                    header.Add((byte)0x00);
                }
            }
            //198 	— 	Not used.
            header.AddRange(BitConverter.GetBytes((UInt16)0));
            //199 - 234     ASCII   Header Event labels(2 or 4 characters, depending on Word 150).
            for (int i = 0; i < 18; i++)
            {
                // If the header event support 4 character but the label is less
                // We add two null character
                if (i < HeaderEvents.Length)
                {
                    string label = HeaderEvents[i].EventLabel;
                    if (label.Length > 4)
                    {
                        label = label.Substring(0, 4);
                    }
                    else if (label.Length < 4)
                    {
                        label = label.PadRight(4, '\0');
                    }
                    header.AddRange(System.Text.Encoding.ASCII.GetBytes(label));
                }
                // For the unused event we fill them with null character
                else
                {
                    header.AddRange(System.Text.Encoding.ASCII.GetBytes("\0\0\0\0"));
                }
            }
            //235 - 256 	— 	Not used.
            for(int i = 0; i < 22; i++)
            {
                header.AddRange(BitConverter.GetBytes((UInt16)0));
            }
            return header.ToArray();
        }
    }
}
