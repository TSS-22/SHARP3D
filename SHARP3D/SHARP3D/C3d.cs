using SHARP3D.Data.Clean;
using SHARP3D.Header.DataEntity;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;


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
                AnalogframePerFrame = c3dFile.Analog.AnalogframePerFrame,
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

        public void Save(string filePath, string fileName, FileFrameFormat fileFrameFormat = FileFrameFormat.C3D_STANDARD)
        {
            var fs = new FileStream(
                $"{filePath}/{fileName}.c3d",
                FileMode.OpenOrCreate,     // Create if doesn't exist
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.Asynchronous   // Enable async
            );
            fs.Seek(0, SeekOrigin.Begin);

            try 
            {
                byte[] parameters = ParametersToBinaries();
                // It should work all the time because the parameter length is padded with zero
                // So no need for ceiling and float cast, as the division will be round.
                byte[] header = HeaderToBinaries((parameters.Length / 512)); // We add the +1 in the HeaderToBinaries function

                fs.WriteAsync(header);
                fs.WriteAsync(parameters);

                // Compute scale factor.
                // This is another reason to put the computation of this managed parameter and their byte function into their respective class
                // To keep a single source of truth.
                float maximumPointValue = 0;
                foreach (C3dPointTrajectory trajectory in Data.Points)
                {
                    foreach (float? val in trajectory.Point)
                    {
                        if (val == null)
                        {
                            continue;
                        }
                        else if (Math.Abs((float)val) > maximumPointValue)
                        {
                            maximumPointValue = (float)val;
                        }
                    }
                }
                DataToBinaries(
                    maximumPointValue / 32000,
                    fs
                    );
            }
            finally
            {
                fs.Close();
            }
        }

        // This function add the +1 block from the parameter length to compute blocks to data start.
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
            int totalNumberAnalogChannels = Data.Analogs.Length;
            foreach(C3dForceplate forceplate in Data.Forceplates)
            {
                totalNumberAnalogChannels = totalNumberAnalogChannels + forceplate.Channels.Length;
            }
            header.AddRange(BitConverter.GetBytes((UInt16)(Required.Analog.AnalogframePerFrame * totalNumberAnalogChannels)));
            //4   uint16 First frame number of raw data(not used / misleading).
            header.AddRange(BitConverter.GetBytes((UInt16)1));
            //5   uint16 Last frame number of raw data(not used / misleading).
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Point.Frames));
            //6   uint16 Maximum 3D frame interpolation gap.
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Point.MaximumInterpolationGap));
            //7 - 8     float32 Data Scale factor.
            float maximumValue = 0f;
            (float?[,,] allPointsData, _, _) = Data.GetAllPointsData();
            foreach (float? point in allPointsData)
            {
                if (point.HasValue && Math.Abs(point.Value) > maximumValue)
                {
                    maximumValue = Math.Abs(point.Value);
                }
            }
            header.AddRange(BitConverter.GetBytes(-(maximumValue / 32000f)));
            //9   uint16 Number of 512 - byte blocks to the Data Section + 1.
            header.AddRange(BitConverter.GetBytes((UInt16)(blockLengthParameterSection + 2))); // Because C3D doesn't start at index 0
            //10  uint16 Analog Frames per Data Frame.
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Analog.AnalogframePerFrame));
            //11 - 12   float32     3D Point Data acquisition rate in Hertz.
            header.AddRange(BitConverter.GetBytes(Required.Point.Rate));
            //13 - 149  — 	Not used.
            for (int i = 0; i < 137; i++)
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
            for (int i = 0; i < 22; i++)
            {
                header.AddRange(BitConverter.GetBytes((UInt16)0));
            }
            return header.ToArray();
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

            int idGroupPoint = 0;
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
                parametersBytes.AddRange(BitConverter.GetBytes((Int16)(2 + 1 + groupDescription.Length)));
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
                        bitsValues.Count != 0? (int)bitsValues.Average() : (int)12,
                        true
                    ));
                    ///////////////////////////////////
                    // ANALOG:DESCRIPTIONS[0-9]*
                    List<string> analogDescriptions = new List<string>();
                    // Start with the forceplate channels
                    foreach(C3dForceplate forceplate in Data.Forceplates)
                    {
                        foreach (C3dAnalogChannel channel in forceplate.Channels)
                        {
                            analogDescriptions.Add(channel.Description);
                        }
                    }
                    // Get the rest of the channels
                    foreach(C3dAnalogChannel channel in Data.Analogs)
                    {
                        analogDescriptions.Add(channel.Description);
                    }
                    int maxDescriptionLength = 0;
                    foreach(string description in analogDescriptions)
                    {
                        if (description.Length > maxDescriptionLength)
                        {
                            maxDescriptionLength = description.Length;
                        }
                    }
                    //// Pad the descriptions
                    for(int i=0; i<analogDescriptions.Count;i++)
                    {
                        if(analogDescriptions[i].Length < maxDescriptionLength)
                        {
                            analogDescriptions[i] = analogDescriptions[i] + new string('\0', maxDescriptionLength - analogDescriptions[i].Length);
                        }
                    }
                    // Put the description array into FORTRAN mode
                    List<char[,]> finalAnalogDescription = new List<char[,]>();
                    List<char[]> bufferAnalogDescriptions = new List<char[]>();
                    int counterAnalogDescription = 0;
                    for(int i =  0; i<analogDescriptions.Count; i++)
                    {
                        bufferAnalogDescriptions.Add(analogDescriptions[i].ToCharArray());
                        counterAnalogDescription++;

                        if ((counterAnalogDescription >= 255)
                            || ((i == analogDescriptions.Count - 1) && (bufferAnalogDescriptions.Count > 0))
                            )
                        {
                            finalAnalogDescription.Add(bufferAnalogDescriptions.To2DArray());
                            bufferAnalogDescriptions = new List<char[]>();
                            counterAnalogDescription = 0;
                        }
                    }
                    
                    for(int i=0; i< finalAnalogDescription.Count; i++)
                    {
                        parametersBytes.AddRange(Parameter2DStringToBinary(
                        idGroup,
                        i == 0 ? "DESCRIPTIONS" : $"DESCRIPTIONS{i}",
                        $"Stores documentation about each of the individual analog channels from analog channel id {i*255}, to id{i*255 + finalAnalogDescription[i].Length}.",
                        finalAnalogDescription[i],
                        false
                        ));
                    }

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
                    ////////////////////////////////////////
                    // ANALOG:LABELS[0-9]*
                    List<string> analogLabels = new List<string>();
                    // Start with the forceplate channels
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        foreach (C3dAnalogChannel channel in forceplate.Channels)
                        {
                            analogLabels.Add(channel.Label);
                        }
                    }
                    // Get the rest of the channels
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        analogLabels.Add(channel.Label);
                    }
                    // Get max label length
                    int maxLabelLength = 0;
                    foreach (string label in analogLabels)
                    {
                        if (label.Length > maxLabelLength)
                        {
                            maxLabelLength = label.Length;
                        }
                    }
                    //// Pad the labels
                    for (int i = 0; i < analogLabels.Count; i++)
                    {
                        if (analogLabels[i].Length < maxLabelLength)
                        {
                            analogLabels[i] = analogLabels[i] + new string('\0', maxLabelLength - analogLabels[i].Length);
                        }
                    }
                    // Put the labels array into FORTRAN mode
                    List<char[,]> finalAnalogLabels = new List<char[,]>();
                    List<char[]> bufferAnalogLabels = new List<char[]>();
                    int counterAnalogLabel = 0;
                    for (int i = 0; i < analogLabels.Count; i++)
                    {
                        bufferAnalogLabels.Add(analogLabels[i].ToCharArray());
                        counterAnalogLabel++;

                        if ((counterAnalogLabel >= 255)
                            || ((i == analogLabels.Count - 1) && (bufferAnalogLabels.Count > 0))
                            )
                        {
                            finalAnalogLabels.Add(bufferAnalogLabels.To2DArray());
                            bufferAnalogLabels = new List<char[]>();
                            counterAnalogLabel = 0;
                        }
                    }

                    for (int i = 0; i < finalAnalogLabels.Count; i++)
                    {
                        parametersBytes.AddRange(Parameter2DStringToBinary(
                        idGroup,
                        i == 0? "LABELS" : $"LABELS{i}",
                        $"Stores the unique labels of each of the individual analog channels from analog channel id {i * 255}, to id{i * 255 + finalAnalogLabels[i].Length}.",
                        finalAnalogLabels[i],
                        false
                        ));
                    }

                    ////////////////////////////////
                    // ANALOG:OFFSET[0-9]*
                    int counter = 0;
                    List<int[]> analogOffsetArrays = new List<int[]>();
                    List<int> bufferOffset = new List<int>();

                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        foreach (C3dAnalogChannel channel in forceplate.Channels)
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
                    }
                    foreach (C3dAnalogChannel channel in Data.Analogs)
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
                    if (bufferOffset.Count > 0)
                    {
                        analogOffsetArrays.Add(bufferOffset.ToArray());
                    }
                    for(int i=0; i< analogOffsetArrays.Count; i++)
                    {
                        parametersBytes.AddRange(Parameter1DArrayToBinary(
                            idGroup,
                            i == 0 ? "OFFSET": $"OFFSET{i}",
                            $"Store array of integer values that are subtracted from each analog measurement before the individual ANALOG:SCALE scaling factors are applied. From analog channel id{i * 255}, to id {i * 255 + analogOffsetArrays[i].Length}.",
                            analogOffsetArrays[i],
                            false
                            ));
                    }
                    
                    ///////////////////////////////
                    // ANALOG:RATE
                    List<float> ratesValues = new List<float>();
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        for (int i = 0; i < forceplate.Channels.Length; i++)
                        {
                            ratesValues.Add(forceplate.Channels[i].Rate);
                        }
                    }
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        ratesValues.Add(channel.Rate);
                    }
                    
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "RATE",
                        "Stores the sample rate at which the analog data was collected in samples per second.",
                        ratesValues.Count !=0 ? ratesValues.Average():0,
                        true
                    ));

                    /////////////////////////////////////
                    // ANALOG:SCALE[0-9]*
                    int counterScale = 0;
                    List<float[]> analogScaleArrays = new List<float[]>();
                    List<float> bufferScale = new List<float>();

                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        foreach (C3dAnalogChannel channel in forceplate.Channels)
                        {
                            bufferScale.Add(channel.Scale);
                            counterScale++;
                            if (counterScale >= 255)
                            {
                                analogScaleArrays.Add(bufferScale.ToArray());
                                bufferScale = new List<float>();
                                counterScale = 0;
                            }
                        }
                    }

                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        bufferScale.Add(channel.Scale);
                        counterScale++;
                        if (counterScale >= 255)
                        {
                            analogScaleArrays.Add(bufferScale.ToArray());
                            bufferScale = new List<float>();
                            counter = 0;
                        }
                    }
                    if (bufferScale.Count > 0)
                    {
                        analogScaleArrays.Add(bufferScale.ToArray());
                    }
                    for (int i = 0; i < analogScaleArrays.Count; i++) 
                    {
                        parametersBytes.AddRange(Parameter1DArrayToBinary(
                            idGroup,
                            i == 0 ? "SCALE" : $"SCALE{i}",
                            $"Stores array of floating-point values that are applied together with the ANALOG:GEN_SCALE parameter value to convert the analog data to physical world values.From analog channel id{i * 255}, to id {i * 255 + analogScaleArrays[i].Length}.",
                            analogScaleArrays[i],
                            true
                            ));
                    }

                    //////////////////////////////////////
                    // ANALOG:UNITS[0-9]*
                    List<string> analogUnits = new List<string>();
                    // Start with the forceplate channels
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        foreach (C3dAnalogChannel channel in forceplate.Channels)
                        {
                            analogUnits.Add(channel.Units);
                        }
                    }
                    // Get the rest of the channels
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        analogUnits.Add(channel.Units);
                    }
                    // Get the longest unit length
                    int maxUnitLength = 0;
                    foreach (string unit in analogUnits)
                    {
                        if (unit.Length > maxUnitLength)
                        {
                            maxUnitLength = unit.Length;
                        }
                    }
                    // Pad the units
                    for (int i = 0; i < analogUnits.Count; i++)
                    {
                        if (analogUnits[i].Length < maxUnitLength)
                        {
                            analogUnits[i] = analogUnits[i] + new string('\0', maxUnitLength - analogUnits[i].Length);
                        }
                    }
                    // Put the units array into FORTRAN mode
                    List<char[,]> finalAnalogUnits = new List<char[,]>();
                    List<char[]> bufferAnalogUnits = new List<char[]>();
                    int counterAnalogUnit = 0;
                    for (int i = 0; i < analogUnits.Count; i++)
                    {
                        bufferAnalogUnits.Add(analogUnits[i].ToCharArray());
                        counterAnalogUnit++;

                        if ((counterAnalogUnit >= 255) 
                            || ((i == analogUnits.Count - 1) && (bufferAnalogUnits.Count > 0))
                            )
                        {
                            
                            finalAnalogUnits.Add(bufferAnalogUnits.To2DArray());
                            bufferAnalogUnits = new List<char[]>();
                            counterAnalogUnit = 0;
                        }
                    }

                    for (int i = 0; i < finalAnalogUnits.Count; i++)
                    {
                        parametersBytes.AddRange(Parameter2DStringToBinary(
                        idGroup,
                        i == 0 ? "UNITS" : $"UNITS{i}",
                        $"Stores the units of each of the individual analog channels from analog channel id {i * 255}, to id{i * 255 + finalAnalogUnits[i].Length}.",
                        finalAnalogUnits[i],
                        false
                        ));
                    }
                    /////////////////////////////////////////
                    // ANALOG:USED
                    int analogUsed =0;
                    foreach (C3dForceplate forceplate in Data.Forceplates)
                    {
                        for (int i = 0; i < forceplate.Channels.Length; i++)
                        {
                            analogUsed++;
                        }
                    }
                    foreach (C3dAnalogChannel channel in Data.Analogs)
                    {
                        analogUsed++;
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

                    /////////////////////////////////
                    // FORCE_PLATFORM:CAL_MATRIX
                    if (forceplateUsed != 0)
                    {
                        List<float[,]> calmatValues = new List<float[,]>();
                        foreach (C3dForceplate forceplate in Data.Forceplates)
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
                        int[,] forceplateChannelValues = new int[8, Data.Forceplates.Length]; // We default to 8 values, without testing if TYPE-3 plates are present. it should work and is easier.
                        int idChannel = 0;
                        for (int i = 0; i < Data.Forceplates.Length; i++)
                        {
                            for (int j = 0; j < Data.Forceplates[i].Channels.Length; j++)
                            {
                                forceplateChannelValues[j, i] = idChannel + 1; // Because C3D is 1 based index
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
                        float[,] originValues = new float[3, Data.Forceplates.Length];
                        for (int i = 0; i < Data.Forceplates.Length; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                originValues[j, i] = Data.Forceplates[i].Origin[j];
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
                        foreach (C3dForceplate forceplate in Data.Forceplates)
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

                        ///////////////////////////
                        // FORCE_PLATFORM:ZERO
                        int[] zeroValues = new int[2];
                        foreach (C3dForceplate forceplate in Data.Forceplates)
                        {
                            zeroValues[0] = forceplate.Zero.Item1 + 1;
                            zeroValues[1] = forceplate.Zero.Item2 + 1;
                        }
                        parametersBytes.AddRange(Parameter1DArrayToBinary(
                            idGroup,
                            "ZERO",
                            "Specify the range of 3D data frame numbers that may be used to provide a baseline for the force platform measurements.",
                            zeroValues,
                            false
                            ));
                    }

                }
                else if (groups[idGroup].Name == "POINT")
                {
                    idGroupPoint = idGroup;
                    /////////////////////////////////////////////////
                    // POINT:DESCRIPTIONS[0-9]*
                    List<string> pointDescriptions = new List<string>();

                    foreach (C3dPointTrajectory trajectory in Data.Points)
                    {
                        pointDescriptions.Add(trajectory.Description);
                    }
                    int maxDescriptionLength = 0;
                    foreach (string description in pointDescriptions)
                    {
                        if (description.Length > maxDescriptionLength)
                        {
                            maxDescriptionLength = description.Length;
                        }
                    }
                    //// Pad the descriptions
                    for (int i = 0; i < pointDescriptions.Count; i++)
                    {
                        if (pointDescriptions[i].Length < maxDescriptionLength)
                        {
                            pointDescriptions[i] = pointDescriptions[i] + new string('\0', maxDescriptionLength - pointDescriptions[i].Length);
                        }
                    }
                    // Put the description array into FORTRAN mode
                    List<char[,]> finalPointDescriptions = new List<char[,]>();
                    List<char[]> bufferPointDescriptions = new List<char[]>();
                    int counterPointDescription = 0;
                    for (int i = 0; i < pointDescriptions.Count; i++)
                    {
                        bufferPointDescriptions.Add(pointDescriptions[i].ToCharArray());
                        counterPointDescription++;

                        if ((counterPointDescription >= 255) 
                            || ((i == pointDescriptions.Count - 1) && (bufferPointDescriptions.Count > 0))
                            )
                        {
                            finalPointDescriptions.Add(bufferPointDescriptions.To2DArray());
                            bufferPointDescriptions = new List<char[]>();
                            counterPointDescription = 0;
                        }
                    }

                    for (int i = 0; i < finalPointDescriptions.Count; i++)
                    {
                        parametersBytes.AddRange(Parameter2DStringToBinary(
                        idGroup,
                        i == 0 ? "DESCRIPTIONS" : $"DESCRIPTIONS{i}",
                        $"Stores documentation about each of the individual 3D Point Trajectories from trajectory id {i * 255}, to id{i * 255 + finalPointDescriptions[i].Length}.",
                        finalPointDescriptions[i],
                        false
                        ));
                    }
                    ////////////////////////////////
                    // POINT:FRAMES
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "FRAMES",
                        "Stores the number of 3D data frames that are recorded in the C3D file.",
                        (float)Required.Point.Frames,
                        true
                    ));
                    //////////////////////////////
                    // POINT:LABELS[0-9]*
                    List<string> pointLabels = new List<string>();
                    // Get the rest of the channels
                    foreach (C3dPointTrajectory trajectory in Data.Points)
                    {
                        pointLabels.Add(trajectory.Label);
                    }
                    // Get max label length
                    int maxLabelLength = 0;
                    foreach (string label in pointLabels)
                    {
                        if (label.Length > maxLabelLength)
                        {
                            maxLabelLength = label.Length;
                        }
                    }
                    //// Pad the labels
                    for (int i = 0; i < pointLabels.Count; i++)
                    {
                        if (pointLabels[i].Length < maxLabelLength)
                        {
                            pointLabels[i] = pointLabels[i] + new string('\0', maxLabelLength - pointLabels[i].Length);
                        }
                    }
                    // Put the labels array into FORTRAN mode
                    List<char[,]> finalPointLabels = new List<char[,]>();
                    List<char[]> bufferPointLabels = new List<char[]>();
                    int counterPointLabel = 0;
                    for (int i = 0; i < pointLabels.Count; i++)
                    {
                        bufferPointLabels.Add(pointLabels[i].ToCharArray());
                        counterPointLabel++;

                        if ((counterPointLabel >= 255)
                            || ((i == pointLabels.Count - 1) && (bufferPointLabels.Count > 0))
                            )
                        {

                            finalPointLabels.Add(bufferPointLabels.To2DArray());
                            bufferPointLabels = new List<char[]>();
                            counterPointLabel = 0;
                        }
                    }

                    for (int i = 0; i < finalPointLabels.Count; i++)
                    {
                        parametersBytes.AddRange(Parameter2DStringToBinary(
                        idGroup,
                        i == 0 ? "LABELS" : $"LABELS{i}",
                        $"Stores the unique labels of each of the individual 3D Points Trajectories. From trajectory id {i * 255}, to id{i * 255 + finalPointLabels[i].Length}.",
                        finalPointLabels[i],
                        false
                        ));
                    }
                    // "LONG_FRAMES", Don't bother till I have the different save options
                    //////////////////////////////////
                    // POINT:RATE
                    parametersBytes.AddRange(ParameterScalarToBinary(
                        idGroup,
                        "RATE",
                        "Stores the 3D sample rate of the data contained within the C3D file in samples per second.",
                        (float)Required.Point.Rate,
                        true
                    ));
                    ///////////////////////////////
                    // POINT:SCALE
                    float maximumPointValue = 0;
                    foreach (C3dPointTrajectory trajectory in Data.Points) 
                    {
                        foreach(float? val in trajectory.Point)
                        {
                            if (val == null)
                            {
                                continue;
                            } 
                            else if (Math.Abs((float)val) > maximumPointValue)
                            {
                                maximumPointValue = (float)val;
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
                    byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(parameter.Name);
                    // Name Length
                    parametersBytes.Add(parameter.Locked ?
                    (byte)(-nameBytes.Length) : (byte)nameBytes.Length
                    );

                    // ID
                    parametersBytes.Add((byte)(idGroup+1));

                    // Name
                    parametersBytes.AddRange(nameBytes);

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
                        // Flatten the array, so we can just work on one index.
                        Array flattenedData;
                        switch (parameterDataType)
                        {
                            case -1: // Char and string
                                flattenedData = parameter.Data.Cast<char>().ToArray();
                                break;
                            case 1: // Byte
                                flattenedData = parameter.Data.Cast<byte>().ToArray();
                                break;
                            case 2: // Int16
                                flattenedData = parameter.Data.Cast<int>().ToArray();
                                break;
                            case 4: // Float
                                flattenedData = parameter.Data.Cast<float>().ToArray();
                                break;
                            default:
                                throw new Exception($"Unsupported data type {dataType} for parameter {parameter.Name}");
                        }
                        for (int i = 0; i < parameter.Data.Length; i++)
                        {
                            // Put the datay in bytes
                            switch (parameterDataType)
                            {
                                case -1: // Char and string
                                    parameterData.Add(
                                        (byte)(flattenedData.GetValue(i) as char? ?? throw new NullReferenceException(""))
                                    );
                                    break;
                                case 1: // Byte
                                    parameterData.Add(
                                        flattenedData.GetValue(i) as byte? ?? throw new NullReferenceException("")
                                    );
                                    break;
                                case 2: // Int16
                                    parameterData.AddRange(
                                        BitConverter.GetBytes(
                                        (Int16)(flattenedData.GetValue(i) as int? ?? throw new NullReferenceException(""))
                                        )
                                    );
                                    break;
                                case 4: // Float
                                    parameterData.AddRange(
                                        BitConverter.GetBytes(
                                        flattenedData.GetValue(i) as float? ?? throw new NullReferenceException("")
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
                    int pointerToNext = 2 + 1 + 1 + parameterDimensionsLength.Length + parameterData.Count + 1 + parameterDescription.Length;
                    
                    // Adding all the bytes together
                    parametersBytes.AddRange(BitConverter.GetBytes((UInt16)pointerToNext));
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
            string datastartName = "DATA_START";
            int datastartNameLength = System.Text.Encoding.ASCII.GetBytes(datastartName).Length;
            string datastartDescription = "An unsigned 16-bit integer value used as a pointer that points to the first block of the data section. Its value is in blocks (512 bytes).";
            int datastartDescriptionLength = System.Text.Encoding.UTF8.GetBytes(datastartDescription).Length;

            int datastartByteLength = 
                1 // Name Length
                + 1 // ID
                + datastartNameLength // data_start name length
                + 2 // Pointer to next
                + 1 // Data type
                + 1 // Dim number
                + 2 // Data
                + 1 // Description length
                + datastartDescriptionLength;

            int totalParameterBytesLength = datastartByteLength + parametersBytes.Count;

            int datastartValue = (int)Math.Ceiling((float)totalParameterBytesLength / (float)512) + 1;
            
            parametersBytes.Add((byte)(-datastartNameLength)); // Name length. Negative because POINT:DATA_START is locked
            parametersBytes.Add((byte)idGroupPoint); // ID. Related to POINT group
            parametersBytes.AddRange(System.Text.Encoding.ASCII.GetBytes(datastartName)); // Name
            parametersBytes.AddRange(BitConverter.GetBytes((UInt16)0)); // Pointer to next
            parametersBytes.Add((byte)2); // Data type
            parametersBytes.Add((byte)0); // Dimension number
            parametersBytes.AddRange(BitConverter.GetBytes((Int16)datastartValue));
            parametersBytes.Add((byte)datastartDescriptionLength);
            parametersBytes.AddRange(System.Text.Encoding.UTF8.GetBytes(datastartDescription));

            // IMPORTANT
            // Pads with zero to finish the blocks so it is a multiple of 512 bytes.
            parametersBytes.AddRange(new byte[datastartValue * 512 - parametersBytes.Count]);

            // Put the length of the parameter section in the third byte
            parametersBytes[2] = (byte)datastartValue;
         
            return parametersBytes.ToArray();
        }

        public void DataToBinaries(
            float scaleFactor,
            FileStream fs
            )
        {
            for(int idFrame = 0; idFrame < Required.Point.Frames; idFrame++)
            {
                foreach (C3dPointTrajectory trajectory in Data.Points)
                {
                    // X Y Z values
                    // X
                    float? tempCoordinate = trajectory.Point[idFrame, 0];
                    float x = tempCoordinate != null ? (float)tempCoordinate : 0;
                    // Y
                    tempCoordinate = trajectory.Point[idFrame, 1];
                    float y = tempCoordinate != null ? (float)tempCoordinate : 0;
                    // Z
                    tempCoordinate = trajectory.Point[idFrame, 2];
                    float z = tempCoordinate != null ? (float)tempCoordinate : 0;

                    fs.WriteAsync(BitConverter.GetBytes(x));
                    fs.WriteAsync(BitConverter.GetBytes(y));
                    fs.WriteAsync(BitConverter.GetBytes(z));

                    // BIT 1: Camera mask and IsValid
                    byte bit1 = 0b00000000;
                    if(
                        trajectory.Point[idFrame, 0] == null
                        || trajectory.Point[idFrame, 1] == null
                        || trajectory.Point[idFrame, 2] == null)
                    {
                        bit1 = 0b10000000;
                    }
                    for(int idCam = 0; idCam < 7; idCam++) // We stickt ot 7 camera only at the moment as per C3D convention
                    {
                        if(trajectory.CameraMask[idFrame, idCam] == true)
                        {
                            bit1 |= (byte)(1 << (idCam));
                        }
                    }

                    // BIT 2: Residual. We need black magic here lol Or the residuals will be corrupted
                    byte bit2 = (trajectory.Residual[idFrame] != null ?
                        (byte)(trajectory.Residual[idFrame] / scaleFactor)
                        : (byte) 0b00000000);

                    // Aggregate the bits in a Int16
                    Int16 int16ByteAggregate = BitConverter.ToInt16(new byte[] { bit1, bit2 });
                    float float32ByteAggregate = (float)int16ByteAggregate;
                    // Write the byte 4
                    fs.WriteAsync(BitConverter.GetBytes(float32ByteAggregate));

                }
                for (int idAnalogFrame = 0; idAnalogFrame < Required.Analog.AnalogframePerFrame; idAnalogFrame++)
                {
                    foreach (C3dForceplate forceplate in Data.Forceplates) 
                    {
                        for (int idChannel = 0; idChannel < forceplate.Channels.Length; idChannel++)
                        {
                            fs.WriteAsync(BitConverter.GetBytes(forceplate.Channels[idChannel].DescaleData(forceplate.Channels[idChannel].Data[idFrame * Required.Analog.AnalogframePerFrame + idAnalogFrame], Required.Analog.GeneralScale)));
                        }
                    }
                    for(int idChannel = 0; idChannel < Data.Analogs.Length; idChannel++)
                    {
                        fs.WriteAsync(BitConverter.GetBytes(Data.Analogs[idChannel].DescaleData(Data.Analogs[idChannel].Data[idFrame * Required.Analog.AnalogframePerFrame + idAnalogFrame], Required.Analog.GeneralScale)));
                    }
                }
            }
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
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dataType // Data bytes (because scalar)
                            + 1 // Description length
                            + descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(BitConverter.GetBytes((Int16)value));
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
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + 4 // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
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
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + 1 // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
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
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                2 // Pointer to next
                + 1 // Data type
                + 1 // dimension number. There is no data length because it is a scalar
                + 1 // Data bytes
                + 1 // Description length
                + descriptionLength // Description bytes
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
            byte dimensionsLength = (byte)value.Length;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                2 // Pointer to next is an int
                + 1  // Data type
                + 1 // Dimension number
                + 1 // Dimension length
                + valueByte.Length  // Value bytes
                + 1  // Description length
                + descriptionLength) // Description bytes 
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
                //byte dimensionLength = (byte)dataBytes.Count;
                byte dimensionLength = (byte)arrayData.Length;
                // Description Length
                byte descriptionLength = (byte)descriptionBytes.Length;
                // Pointer to next
                byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                                2 // Pointer to next
                                + 1 // Data type
                                + 1 // dimension number. There is no data length because it is a scalar
                                + 1 // dimension length
                                + dataBytes.Count // Data bytes
                                + 1 // Description length
                                + descriptionLength // Description bytes
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
            byte dataType = 2;
            // Dimensions numbers
            byte dimensionNumber = 1;
            //Dimension length
            //byte dimensionLength = (byte)dataBytes.Count;
            byte dimensionLength = (byte)arrayData.Length;
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + 1 // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
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
            byte dataType = 1;
            // Dimensions numbers
            byte dimensionNumber = 2;
            //Dimension length
            byte[] dimensionLength = new byte[] { (byte)arrayData.GetLength(1), (byte)arrayData.GetLength(0)};
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
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
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
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
            byte[] dimensionLength = new byte[] { (byte)arrayData.GetLength(1), (byte)arrayData.GetLength(0) };
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
                            )); parameterBytes.AddRange(pointerToNext);
            
            parameterBytes.Add(dataType);
            parameterBytes.Add(dimensionNumber);
            parameterBytes.AddRange(dimensionLength);
            parameterBytes.AddRange(dataBytes);
            parameterBytes.Add(descriptionLength);
            parameterBytes.AddRange(descriptionBytes);

            return parameterBytes.ToArray();
        }

        public byte[] Parameter2DStringToBinary(
            int idGroup,
            string name,
            string description,
            char[,] arrayData,
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
                    dataBytes.AddRange(System.Text.Encoding.UTF8.GetBytes(new char[] { arrayData[i, j] }));
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
            byte dataType;
            unchecked { dataType = (byte)-1; }
            // Dimensions numbers
            byte dimensionNumber = 2;
            //Dimension length
            byte[] dimensionLength = new byte[] { (byte)arrayData.GetLength(1), (byte)arrayData.GetLength(0) };
            // Description Length
            byte descriptionLength = (byte)descriptionBytes.Length;
            // Pointer to next
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
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
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
                            2 // Pointer to next
                            + 1 // Data type
                            + 1 // dimension number. There is no data length because it is a scalar
                            + dimensionLength.Length // dimension length
                            + dataBytes.Count // Data bytes
                            + 1 // Description length
                            + descriptionLength // Description bytes
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
            byte[] pointerToNext = BitConverter.GetBytes((UInt16)(
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

        
    }
}
