using SHARP3D.Data;
using SHARP3D.Data.DataEntity;
using SHARP3D.Exceptions;
using SHARP3D.Header.DataEntity;
using SHARP3D.Parameter;
using SHARP3D.Parameter.DataEntity;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SHARP3D.Test")]
[assembly: InternalsVisibleTo("SHARP3D.Explorer")] // To remove for production
namespace SHARP3D
{
    /// <summary>
    /// Represents a C3D file, providing methods for processing headers, parameters, loading, saving, and binary conversion.
    /// </summary>
    /// <remarks>
    /// This class encapsulates all the functionality needed to read, parse, and manipulate C3D files,
    /// including header information, parameters, and data frames.
    /// </remarks>
    public class C3dFile
    {
        /// <summary>
        /// The path of the C3D File.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The length of the C3D File in bytes.
        /// </summary>
        public long FileLength { get; set; }

        /// <summary>
        /// Specifies the type of processor that was used to create the C3D file.
        /// </summary>
        public ProcessorType ProcessorFile { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;

        /// <summary>
        /// Gets or sets the type of processor used by the host system.
        /// </summary>
        public ProcessorType ProcessorHost { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;

        /// <summary>
        /// Gets or sets the data type used in the C3D file.
        /// </summary>
        public DataType DataTypeFile { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the parameter section in the C3D file.
        /// </summary>
        public int PointerParameterSection { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the data section in the C3D file.
        /// </summary>
        public int PointerDataSection { get; set; }

        /// <summary>
        /// Gets or sets the header information of the C3D file.
        /// </summary>
        public C3dFileHeader Header { get; set; } = new C3dFileHeader();

        /// <summary>
        /// Gets or sets the list of parameter groups in the C3D file.
        /// </summary>
        public List<C3dFileParameterGroup> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the collection of parameters in the C3D file.
        /// </summary>
        public C3dFileParameterCollection ParameterCollection { get; set; }

        
        public C3dFileParameterPoint Point { get; set; }
        public C3dFileParameterAnalog Analog { get; set; }
        public C3dFileParameterForceplate Forceplate { get; set; }

        /// <summary>
        /// Gets or sets the data contained in the C3D file.
        /// </summary>
        public C3dFileData Data { get; set; }

        /// <summary>
        /// Centralize the values needed to extract the data from the C3D file.
        /// </summary>
        /// <remarks>
        /// It is saved as a Class field for testing, and to help work around bad formatting from files at the moment. It might be discarded later or at least rearranged.
        /// </remarks>
        public C3dFileDataContext DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="C3dFile"/> class.
        /// </summary>
        internal C3dFile() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="C3dFile"/> class with the specified file stream.
        /// </summary>
        /// <param name="fileStream">The file stream to read the C3D file from.</param>
        internal C3dFile(FileStream fileStream)
        {
            FilePath = fileStream.Name;
            FileLength = fileStream.Length;
            ProcessorFile = ReadProcessorByte(fileStream); 
            float tempPointScale = GetPointScale(fileStream, ProcessorFile);
            DataTypeFile = tempPointScale < 0 ? DataType.FLOAT32 : DataType.INT16;

            Header = GetHeader(fileStream, ProcessorFile);

            PointerParameterSection = Header.PointerParameterSection;
            PointerDataSection = Header.PointerDataSection;

            Parameters = GetParameters(fileStream, ProcessorFile, PointerParameterSection, PointerDataSection);

            ParameterCollection = new C3dFileParameterCollection(Parameters);

            Analog = SetFileAnalog();
            Point = setFilePoint(Analog.Used, Analog.Rate);
            Analog.SamplesPerFrame = GetAnalogSamplePerFrame(Point.Rate, Analog.Rate);
            
            // We put it last because it needs some values from Analog.
            Forceplate = setFileForceplate();

            int tempAnalogBits = 12;
            (Data, tempAnalogBits) = GetDataAndBit(fileStream, ProcessorFile, DataTypeFile, Header.ScaleFactor);
            
            // Because ANALOG:BITS is not always defined. It needs to be "guesstimated".
            //https://tss-22.github.io/SHARP3D/c3d_docs/parameters/required/analog/analog-bits.html
            if (Analog.Bits == 0)
            {
                Analog.Bits = tempAnalogBits;
            }
            // We update the Frames count to reflect the actual amount of frames in the data.
            // Not the mistake, or the wishful thinking of the entity creating the file.
            Point.Frames = Data.Points.Count;
            // We make the choice of recomputing the scale factor
            Point.Scale = ComputeScaleFactor();
            // It is usefull to have access to such value later on.
            Analog.TotalSamples = Data.Analogs.Count * Analog.SamplesPerFrame;

            fileStream.Close();
        }

        /// <summary>
        /// Opens a C3D file for reading.
        /// </summary>
        /// <param name="filepath">The path to the C3D file.</param>
        /// <returns>A file stream for the C3D file.</returns>
        /// <remarks>
        /// For legacy purposes for the tests. This method should be discarded for production.
        /// </remarks>
        internal static FileStream OpenC3dFile(string filepath)
        {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read);
        }

        internal C3dFileParameterForceplate setFileForceplate()
        {
            C3dFileParameterForceplate fileForceplate = new C3dFileParameterForceplate();
            
            // FORCE_PLATFORM:USED - DONE
            int forceplateUsed = 0;
            try { 
                forceplateUsed = GetParameter("force_platform", "used").Data?.GetValue(0) as int? ?? 0;
            } catch (ParameterNotFoundException ex) 
            {
                Console.Error.WriteLine($"WARNING: Parameter FORCE_PLATFORM:USED not found. No force platform will be assessed. {ex.Message}");
                forceplateUsed = 0;
            }
            fileForceplate.Used = forceplateUsed;
            if (fileForceplate.Used == 0) 
            {
                return fileForceplate;
            }

            //FORCE_PLATFORM:TYPE - DONE
            ForceplateType[] forceplateType = new ForceplateType[forceplateUsed];
            
            for (int idFp = 0; idFp < fileForceplate.Used; idFp++)
            {
                try
                {
                    forceplateType[idFp] = (ForceplateType)(GetParameter("force_platform", "type").Data?.GetValue(idFp) as int? ??
                        throw new NullReferenceException($"Forceplate {idFp} type was not defined. Defaulting to ForceplateType.UNKOWN."));
                }
                catch (Exception ex) when (
                ex is ParameterNotFoundException
                || ex is IndexOutOfRangeException
                || ex is NullReferenceException)
                {
                    forceplateType[idFp] = ForceplateType.UNKOWN;
                    Console.Error.WriteLine(ex.Message);
                }
            }
            fileForceplate.Type = forceplateType;

            //FORCE_PLATFORM:CAL_MATRIX - DONE
            // We make the assumption that, as FORCE_PLATFORM:CAL_MATRIX is an optional Parameter
            // and in the light of the way it is set in the sample file,
            // we think that CAL_MAT value are only added for TYPE-2 or TYPE-4 force plates.
            // We can't be sure because none of the sample files allows us to check that assumptions.
            // Don't go gentle into that good night.
            List<float[,]> forceplateCalMat = new List<float[,]>();
            int idCalMat = 0;
            
            for (int idFp = 0; idFp < fileForceplate.Used; idFp++)
            {
                // Well, a try catch would be nice here
                int calMatColNb =  6;
                int calMatRowNb =  6;

                float[,] calMat = new float[calMatColNb, calMatRowNb];

                if ((forceplateType[idFp] == ForceplateType.TYPE_2) || (forceplateType[idFp] == ForceplateType.TYPE_4))
                {
                    for (int col = 0; col < calMatColNb; col++)
                    {
                        for (int row = 0; row < calMatRowNb; row++)
                        {
                            float temp = calMat[col, row] = col == row ? 1.0f : 0.0f;
                            try
                            {
                                calMat[col, row] = (float)(GetParameter("force_platform", "cal_matrix").Data?.GetValue(col, row, idCalMat) as float? ??
                                    temp);
                            }
                            catch (ParameterNotFoundException ex)
                            {
                                Console.Error.WriteLine(
                                    $"WARNING: Parameter FORCE_PLATFORM:CAL_MATRIX not found for forceplate {idFp}." +
                                    $" Defaulting to identity matrix. {ex.Message}"
                                    );
                                calMat = ArrayUtils.IdentityMatrix(calMatColNb);
                                goto CalMatNotFound;
                            }
                        }
                    }
                    idCalMat++;
                }
                else
                {
                    if (fileForceplate.Type[idFp] == ForceplateType.TYPE_1)
                    {
                        calMat = new float[,] { };
                    }
                    if (fileForceplate.Type[idFp] == ForceplateType.TYPE_3)
                    {
                        calMat = new float[,] { };
                    }
                }

                CalMatNotFound:
                    forceplateCalMat.Add(calMat);
            }
            fileForceplate.CalibrationMatrix = forceplateCalMat.ToArray();
            
            //FORCE_PLATFORM:CORNERS - DONE
            List<float[,]> forceplateCorner = new List<float[,]>();
            
            for (int idFp = 0; idFp < fileForceplate.Used; idFp++)
            {
                float[,] cornerData = new float[4, 3];
                for (int idCoor = 0; idCoor<3; idCoor++)
                {
                    for(int idCorner = 0;idCorner<4; idCorner++)
                    {
                        try
                        {
                            cornerData[idCorner, idCoor] = (float)(GetParameter("force_platform", "corners").Data?.GetValue(idCoor, idCorner, idFp) as float? ??
                                throw new NullReferenceException($"Axis {idCoor} of Corner {idCorner} not advertised for forceplate {idFp}."));
                        }
                        catch (Exception ex) when (
                            ex is ParameterNotFoundException
                            || ex is IndexOutOfRangeException
                            || ex is NullReferenceException) {
                        cornerData[idCorner, idCoor] = 0;
                            Console.Error.WriteLine(ex.Message);
                        }
                    }
                }
                forceplateCorner.Add(cornerData);
            }
            fileForceplate.Corners = forceplateCorner.ToArray();
            

            //FORCE_PLATEFORM:ORIGIN - DONE
            List<float[]> forceplateOrigin = new List<float[]>();

            for (int idFp = 0; idFp < fileForceplate.Used; idFp++)
            {
                float[] originData = new float[3];
                for (int idOrigin = 0; idOrigin < 3; idOrigin++)
                {
                    try
                    {
                        originData[idOrigin] = (float)(GetParameter("force_platform", "origin").Data?.GetValue(idOrigin, idFp) as float? ??
                        throw new NullReferenceException($"ORIGIN Axis {idOrigin} of force plate {idFp}."));
                    }
                    catch (Exception ex) when (
                        ex is ParameterNotFoundException
                        || ex is IndexOutOfRangeException
                        || ex is NullReferenceException)
                    {
                        originData[idOrigin] = 0;
                        Console.Error.WriteLine(ex.Message);
                    }
                }
                forceplateOrigin.Add(originData);
            }
            fileForceplate.Origin = forceplateOrigin.ToArray();

            //FORCE_PLATFORM:ZERO - DONE
            (int,int) forceplateZero = (0,1);
            try
            {
                forceplateZero.Item1 = (int)(GetParameter("force_platform", "zero").Data?.GetValue(0) as int? ?? throw new NullReferenceException($"ZERO beginning frame index error."));
                forceplateZero.Item2 = (int)(GetParameter("force_platform", "zero").Data?.GetValue(1) as int? ?? throw new NullReferenceException($"ZERO last frame index error."));

                if (forceplateZero.Item1 > 0)
                {
                    // We make it so that the index work for most of programming language that start on zero
                    // As C3D first frame is frame 1 and not frame 0.
                    forceplateZero.Item1--;
                    forceplateZero.Item2--;
                }
            }
            catch (Exception ex) when (
                        ex is ParameterNotFoundException
                        || ex is IndexOutOfRangeException
                        || ex is NullReferenceException)
            {
                Console.Error.WriteLine($"WARNING: Parameter FORCE_PLATFORM:ZERO not found or not populated. Defaulting to (0,1). {ex.Message}");
            }
            fileForceplate.Zero = forceplateZero;

            // FORCE_PLATFORM:CHANNEL - DONE
            List<int[]> forceplateChannel = new List<int[]> { };
            
            for (int idFp = 0; idFp < fileForceplate.Used; idFp++)
            {
                int nbChannel;
                int[] tempChannel = new int[] { };
                try
                {
                    // Check how much channels we are expecting from the force plate in regards to its TYPE.
                    
                    if (!Sharp3dConstants.ForceplateChannelNumber.TryGetValue(fileForceplate.Type[idFp], out nbChannel))
                    {
                        throw new ParameterNotFoundException($"Channel number or TYPE error for force plate {idFp} while collecting CHANNEL.");
                    }
                    // Initialize the array to store the channels number
                    tempChannel = new int[nbChannel];
                    for (int idChannel = 0; idChannel < nbChannel; idChannel++)
                    {
                        // We add a "-1" because channels are specified starting 1 not 0...
                        // As C3D, just like the frame index, has its index starting 1.
                        tempChannel[idChannel] = (int)(GetParameter("force_platform", "channel").Data?.GetValue(idChannel, idFp) as int? ?? throw new NullReferenceException($"CHANNEL inedx {idChannel} error for force plate {idFp}.")) - 1;
                    }
                    forceplateChannel.Add(tempChannel);
                }
                catch (Exception ex) when (
                            ex is ParameterNotFoundException
                            || ex is IndexOutOfRangeException
                            || ex is NullReferenceException)
                {
                    // We add an empty array for channels.
                    forceplateChannel.Add(tempChannel);
                    Console.Error.WriteLine($"No channels linked to forceplate {idFp}. Bad channel advertisement." +
                        $" Channels advertised before error: {string.Join(", ", tempChannel)}. {ex.Message}");
                }
            }
            fileForceplate.Channel = forceplateChannel.ToArray();
            

            return fileForceplate;
        }
        internal C3dFileParameterAnalog SetFileAnalog()
        {
            C3dFileParameterAnalog fileAnalog = new C3dFileParameterAnalog();

            try
            {
                fileAnalog.Bits = GetParameter("analog", "bits").Data?.GetValue(0) as int? ?? 12;
            }
            catch (ParameterNotFoundException ex)
            {
                Console.Error.WriteLine($"WARNING: {ex.Message}. Rebuilding from heuristic. See https://tss-22.github.io/SHARP3D/c3d_docs/parameters/required/analog/analog-bits.html.");
                fileAnalog.Bits = 0;
            }

            fileAnalog.GeneralScale = GetAnalogGeneralScale();
            fileAnalog.Rate = GetAnalogRate();
            fileAnalog.Used = GetAnalogUsed();
            fileAnalog.ChannelScale = GetAnalogChannelScale(fileAnalog.Used);
            fileAnalog.Offset = GetAnalogOffset(fileAnalog.Used, GetAnalogFormat());
            fileAnalog.Labels = GetXParameters(fileAnalog.Used, "analog", "labels");
            fileAnalog.Descriptions = GetXParameters(fileAnalog.Used, "analog", "descriptions");
            fileAnalog.Units = GetXParameters(fileAnalog.Used, "analog", "units");
            fileAnalog.SamplesPerFrame = 0;
            return fileAnalog;
        }

        internal C3dFileParameterPoint setFilePoint(int analogUsed, float analogRate)
        {
            C3dFileParameterPoint filePoint = new C3dFileParameterPoint();

            filePoint.Frames = GetRightAmountOfFrames();
            filePoint.Rate = GetParameter("point", "rate").Data?.GetValue(0) as float? ?? 0f;
            filePoint.Scale = Header.ScaleFactor;
            
            // See GetRightAmountMarkerPerFrame. Simple answer: some people fucked up and now we have to go through hoops to make it work reliably.
            int markersPerFrame = GetParameter("point", "used").Data?.GetValue(0) as int? ?? 0;
            filePoint.Used = GetRightAmountMarkerPerFrame(
                filePoint.Frames,
                analogUsed,
                GetAnalogSamplePerFrame(filePoint.Rate, analogRate),
                    Header.MarkersPerFrame,
                    markersPerFrame,
                    PointerDataSection,
                    FileLength,
                    DataTypeFile
                    );
            filePoint.Labels = GetXParameters(filePoint.Used, "point", "labels");
            filePoint.Descriptions = GetXParameters(filePoint.Used, "point", "descriptions");
            try
            {
                char[]? tempUnits = GetParameter("point", "units").Data as char[];
                if (tempUnits == null)
                {
                    throw new ParameterNotFoundException("POINT:UNITS is not populated.");
                }
                else
                {
                    filePoint.Units = new string(tempUnits).Trim();
                }
            }
            catch(ParameterNotFoundException ex)
            {
                Console.Error.WriteLine($"WARNING: {ex.Message}. Defaulting to default C3D Point units: 'mm'");
                filePoint.Units = "mm";
            }
            

                return filePoint;
        }

        internal float GetAnalogRate()
        {
            try
            {
                return GetParameter("analog", "rate").Data?.GetValue(0) as float? ?? 0f; // Contradiction in the C3D documentation. Should have put more info, I forgot what it was.
            } // Seems alright to me.
            catch (ParameterNotFoundException ex) 
            {
                Console.Error.WriteLine("WARNING: No ANALOG:RATE parameter found. Defaulting to 0 Hz for analog data.");
                return 0f;
            }   
        }

        internal int GetAnalogUsed()
        {
            try 
            { 
                return GetParameter("analog", "used").Data?.GetValue(0) as int? ?? 0; 
            } 
            catch (ParameterNotFoundException ex) 
            {
                Console.Error.WriteLine("WARNING: No ANALOG:USED parameter found. Defaulting to 0 analog channels.");
                return 0;
            }
        }

        internal float GetAnalogGeneralScale()
        {
            try 
            {
                return GetParameter("analog", "gen_scale").Data?.GetValue(0) as float? ?? 1f; 
            } catch (ParameterNotFoundException ex) 
            {
                return 1f;
            }
        }

        internal float[] GetAnalogChannelScale(int analogUsed)
        {
            float[] tempAnalogChannelScale = new float[] { 0f };
            try { tempAnalogChannelScale = GetParameter("analog", "scale").Data as float[] ?? new float[] { 0f }; } catch (ParameterNotFoundException ex) { }

            float[] analogChannelScale;
            if (tempAnalogChannelScale.Length >= analogUsed)
            {
                analogChannelScale = tempAnalogChannelScale.Take(analogUsed).ToArray();
            }
            else // Some files don't have enough ANALOG:SCALE_CHANNEL. They seems to only have 1 as the scale factor, hence we just add 1 for the missing indexes.
            {

                float[] paddedArray = new float[analogUsed];

                // Copy the original values
                Array.Copy(tempAnalogChannelScale, paddedArray, tempAnalogChannelScale.Length);

                // Fill the remaining positions with 1
                for (int i = tempAnalogChannelScale.Length; i < analogUsed; i++)
                {
                    paddedArray[i] = 1f;
                }
                analogChannelScale = paddedArray;
            }

            return analogChannelScale;
        }

        internal AnalogFormatFlag GetAnalogFormat()
        {
            // That's the default so we don't care if it is anything else than unsigned. If we can find better strategy than the one from C3D User guide, we will implement it here.
            AnalogFormatFlag analogFormat = AnalogFormatFlag.SIGNED;

            Array analogFormatValue = Sharp3dConstants.SignedArrayString;
            try
            {
                analogFormatValue = GetParameter("analog", "format").Data;
            }
            catch (ParameterNotFoundException ex)
            {
                Console.Error.WriteLine("WARNING: No ANALOG:FORMAT parameter found. Defaulting to SIGNED format for analog data.");
            }

            if (analogFormatValue == Sharp3dConstants.UnsignedArrayString)
            {
                analogFormat = AnalogFormatFlag.UNSIGNED;
            }

            return analogFormat;
        }

        internal int[] GetAnalogOffset(int analogUsed, AnalogFormatFlag analogFormat)
        {
            // Some software have the analogoff set as a float.
            //int analogOffset = 0;
            int[] analogOffset = new int[analogUsed];
            try
            {
                analogOffset = GetParameter("analog", "offset").Data?
                    .OfType<object>()
                    .Select(obj => Convert.ToInt32(obj))
                    .ToArray() ?? Array.Empty<int>();

            }
            catch (IndexOutOfRangeException) { }
            catch (ParameterNotFoundException ex) { }
            return analogOffset;
        }

        internal string[] GetXParameters(int used, string group, string parameter)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            // For POINT: and ANALOG:
            // LABELS, DESCRIPTIONS and UNITS
            // Check the length of group used.
            // Go by chunk of 255 used values and look for the adequate GROUP:PARAMETERX.
            int numberOfParameters = (int)Math.Ceiling((double)used / 255);
            // Create place holder and associated global index for ease of use.
            string[] analogUnits = new string[used];
            int paramIndex = 0;
            int paramLeft = used;
            bool isLastParam = false;

            for (int i = 0; i < numberOfParameters; i++)
            {
                // Check if this is the last parameter number to check
                if (paramLeft <= 255)
                {
                    isLastParam = true;
                }
                // Get the number of parameter to extract
                int paramInBatchToDo = isLastParam ? paramLeft : 255;

                // Get the right parameter name
                string parameterName = $"{parameter}{i + 1}";
                if (i == 0)
                {
                    parameterName = parameter;
                }

                // Process Parameters
                try
                {
                    char[,]? param = GetParameter(group, parameterName).Data as char[,];
                    if (param != null)
                    {
                        // Check if I have the right number of parameters instance (second dimension of the char array).
                        // Some files have too much analog channels vs what is actually used. 
                        int paramInBatch = param.GetLength(1) >= paramInBatchToDo? paramInBatchToDo : param.GetLength(1);

                        for (int j = 0; j < paramInBatch; j++)
                        {
                            List<char> tempCharParam = new List<char> { };
                            for (int k = 0; k < param.GetLength(0); k++)
                            {
                                tempCharParam.Add(param[k, j]);
                            }
                            analogUnits[paramIndex] = new string(tempCharParam.ToArray()).Trim();

                            paramInBatchToDo--;
                            paramLeft--;
                            paramIndex++;
                        }

                        // If there is some left over
                        for (int j = 0; j < paramInBatchToDo; j++)
                        {
                            analogUnits[paramIndex] = $"Channel {paramIndex + 1}. No {parameterName[..^1].ToLower()} provided.";
                            paramLeft--;
                            paramIndex++;
                        }
                    }
                    else
                    {
                        // We throw an exception because the GROUP:PARAMETERX was not populated at all. 
                        // It should not happen though, as it is either gonna be filled, not enough filled, or absent
                        throw new NullReferenceException($"{group.ToUpper()}:{parameterName.ToUpper()} is not populated.");
                    }
                }
                catch (Exception ex) when (ex is ParameterNotFoundException || ex is NullReferenceException)
                {
                    Console.Error.WriteLine($"WARNING: Error with {group.ToUpper()}:{parameterName.ToUpper()}: {ex.Message}. Defaulting to default values for {group.ToUpper()}:{parameterName.ToUpper()} .");
                    for (int j = 0; j < paramInBatchToDo; j++)
                    {
                        analogUnits[paramIndex] = $"Channel {paramIndex + 1}. No {parameterName[..^1].ToLower()} provided.";
                        paramLeft--;
                        paramIndex++;
                    }
                }
            }

            return analogUnits;
        }

        /// <summary>
        /// Reads the header information from the C3D file.
        /// </summary>
        /// <param name="fileStream">The file stream to read from.</param>
        /// <param name="processorFile">The processor type used to create the C3D file.</param>
        /// <returns>A <see cref="C3dFileHeader"/> object containing the header information.</returns>
        internal C3dFileHeader GetHeader(FileStream fileStream, ProcessorType processorFile)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] headerBinaries = ReadHeaderBinaries(fileStream);
            return C3dFileHeader.FromBinaries(headerBinaries, processorFile);
        }

        /// <summary>
        /// Reads the parameters from the C3D file.
        /// </summary>
        /// <param name="fileStream">The file stream to read from.</param>
        /// <param name="processorFile">The processor type used to create the C3D file.</param>
        /// <param name="pointerParameterSection">The pointer to the parameter section.</param>
        /// <param name="pointerDataSection">The pointer to the data section.</param>
        /// <returns>A list of <see cref="C3dParameterGroup"/> objects containing the parameters.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the file stream is not open.</exception>
        internal List<C3dFileParameterGroup> GetParameters(FileStream fileStream, ProcessorType processorFile, int pointerParameterSection, int pointerDataSection)
        {
            if (fileStream == null)
            {
                throw new InvalidOperationException("File stream is not open.");
            }
            return C3dParameterHelper.ParametersFromFileStreams(fileStream, processorFile, pointerParameterSection, pointerDataSection);
        }

        /// <summary>
        /// Loads a C3D file from the specified file path.
        /// </summary>
        /// <param name="filepath">The path to the C3D file.</param>
        /// <returns>A <see cref="C3dFile"/> object representing the loaded file.</returns>
        public static C3dFile LoadFromFile(string filepath)
        {
            FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            
            return new C3dFile(fileStream);
        }

        /// <summary>
        /// Gets the pointer to the parameter section in the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <returns>The pointer to the parameter section.</returns>
        internal static int GetParameterSectionPointer(FileStream c3dStream)
        {
            c3dStream.Seek(0, SeekOrigin.Begin);
            return (c3dStream.ReadByte() - 1) * 512;
        }

        /// <summary>
        /// Gets the pointer to the data section in the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <param name="processor">The processor type used to create the C3D file.</param>
        /// <returns>The pointer to the data section.</returns>
        internal static int GetDataSectionPointer(FileStream c3dStream, ProcessorType processor)
        {
            byte[] pointerToData = new byte[2];
            c3dStream.Seek(16, SeekOrigin.Begin);
            c3dStream.ReadExactly(pointerToData);
            return (C3dBytesConvertor.ToInt(pointerToData, processor) - 1) * 512; // I don't know why you have to substrack 1.
        }

        /// <summary>
        /// Reads the processor type byte from the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <returns>The processor type used to create the C3D file.</returns>
        internal static ProcessorType ReadProcessorByte(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 3, SeekOrigin.Begin);
            return (ProcessorType)c3dStream.ReadByte();
        }

        /// <summary>
        /// Gets the point scale factor from the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <param name="processor">The processor type used to create the C3D file.</param>
        /// <returns>The point scale factor.</returns>
        internal float GetPointScale(FileStream c3dStream, ProcessorType processor)
        {
            byte[] valueBuffer = new byte[4];
            c3dStream.Seek(12, SeekOrigin.Begin);
            c3dStream.ReadExactly(valueBuffer);
            return C3dBytesConvertor.ToFloat(valueBuffer, processor);
        }


        /// <summary>
        /// Reads the header binaries from the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <returns>A byte array containing the header binaries.</returns>
        internal static byte[] ReadHeaderBinaries(FileStream c3dStream)
        {
            byte[] headers = new byte[512];
            c3dStream.ReadExactly(headers, 0, 512);
            return headers;
        }

        /// <summary>
        /// Gets a parameter from the C3D file by its group and parameter name.
        /// </summary>
        /// <param name="groupName">The name of the parameter group.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>A <see cref="C3dFileParameter"/> object representing the requested parameter.</returns>
        public C3dFileParameter GetParameter(string groupName, string parameterName)
        {
            (int,int) indexParameter = ParameterCollection.GetParameterIndex(groupName, parameterName);
            return Parameters[indexParameter.Item1].Parameters[indexParameter.Item2];
        }

        /// <summary>
        /// Reads the data from the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <param name="processor">The processor type used to create the C3D file.</param>
        /// <param name="dataTypeFile">The data type used in the C3D file.</param>
        /// <param name="pointScale">The point scale factor.</param>
        /// <returns>A <see cref="C3dFileData"/> object containing the data. And an int containing the ANALOG:BITS guesstimate.</returns>
        internal (C3dFileData, int) GetDataAndBit(FileStream c3dStream, ProcessorType processor, DataType dataTypeFile, float pointScale)
        {
            // TODO: actually sort the error that can come
            DataContext = new C3dFileDataContext(
                c3dStream: c3dStream,
                processor: processor,
                dataTypeFile: dataTypeFile,
                pointerDataSection: PointerDataSection,
                framesNumber: Point.Frames,
                markersPerFrame: Point.Used,
                pointRate: Point.Rate,
                analogRate: Analog.Rate,
                analogChannels:Analog.Used,
                pointScale: Point.Scale,
                analogGeneralScale: Analog.GeneralScale,
                analogChannelScale: Analog.ChannelScale,
                analogOffset: Analog.Offset,
                analogSamplePerFrame: Analog.SamplesPerFrame,
                analogFormat: GetAnalogFormat()
                );

            return C3dFileDataHelper.FromFileStream(DataContext);
        }


        /// <summary>
        /// Calculates the number of analog samples per 3D frame.
        /// </summary>
        /// <param name="pointRate">
        /// The acquisition rate of the 3D point data, in Hz.
        /// </param>
        /// <param name="analogRate">
        /// The acquisition rate of the analog data, in Hz.
        /// </param>
        /// <returns>
        /// The number of analog samples per 3D frame, as an integer.
        /// </returns>
        /// <remarks>
        /// The result is calculated as the ratio of <paramref name="analogRate"/> to <paramref name="pointRate"/>.
        /// If this ratio is not an integer, the function will try to recover this value from the word 10 of the header, according to the page 27 of the <see href="https://www.c3d.org/docs/C3D_User_Guide.pdf">C3D User guide</see>, as the C3D file format requires this ratio to be an integer.
        /// We started using the division of the analog rate and the point rate due to the descriptions in the Data sectin of the guide and because some files don't have an actual valid value in WORD 10 of the C3D headers, but some badly formatted files require the use of WORD 10 of the C3D headers.
        /// </remarks>
        internal int GetAnalogSamplePerFrame(float pointRate, float analogRate)
        {
            float analogSamplePerFrame;
            if (analogRate > pointRate) 
            {
                analogSamplePerFrame = analogRate / pointRate;
            }
            else
            {
                analogSamplePerFrame = pointRate / analogRate;
            }
            if (Math.Abs(analogSamplePerFrame - (int)analogSamplePerFrame) > 0)
            {
                return Header.AnalogFramePerDataFrame;
            }
            else
            {
                return (int)analogSamplePerFrame;
            }
        }

        /// <summary>
        /// Determines the correct number of frames in a C3D file by attempting to retrieve the value
        /// from multiple parameter sources, as specified in the C3D User Guide.
        /// </summary>
        /// <remarks>
        /// The function follows the C3D User Guide's recommendations for determining the number of frames:
        /// <list type="number">
        ///   <item>
        ///     <description>
        ///       Attempts to retrieve the value from the "point/long_frames" parameter (pages 93-94).
        ///       If successful, returns the value; otherwise, proceeds to the next method.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Attempts to calculate the number of frames using the "trial/actual_start_field" and
        ///       "trial/actual_end_field" parameters (pages 99-101). The number of frames is calculated as:
        ///       <code>
        ///         (lastFrame - firstFrame + 1)
        ///       </code>
        ///       where <c>firstFrame</c> and <c>lastFrame</c> are derived from the parameter values.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Falls back to retrieving the value from the "point/frames" parameter (page 66).
        ///       If all else fails, returns 0 as a default value.
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// The number of frames in the C3D file, as determined by the first successfully retrieved or calculated value.
        /// Returns 0 if no valid value can be determined.
        /// </returns>
        /// <exception cref="ParameterNotFoundException">
        /// Thrown internally if a parameter is not found, but caught and ignored to allow fallback to other methods.
        /// </exception>
        internal int GetRightAmountOfFrames() {
            // As per page 93 and 94 of the C3D User Guide
            try
            {
                int frameNumber = GetPointFrameValue("long_frames");
                if (frameNumber > 0)
                {
                    return frameNumber;
                }
                else
                {
                    throw new C3dBadFrameNumberFormatingException("");
                }
            }
            catch (ParameterNotFoundException e)
            {
                // Do nothing and try the other parameters
            }
            catch (KeyNotFoundException e)
            {
                // Do nothing and try the other parameters
            }
            catch (C3dBadFrameNumberFormatingException e) 
            { 
            
            }

            // As per page 99,100 and 101 of the C3D User Guide.
            try
            {
                C3dFileParameter trialActualStartField = GetParameter("trial", "actual_start_field");
                C3dFileParameter trialActualEndField = GetParameter("trial", "actual_end_field");

                int firstFrame = GetFrameValue(trialActualStartField.Data.GetValue(0)) + GetFrameValue(trialActualStartField.Data.GetValue(1)) * 65535;
                int lastFrame = GetFrameValue(trialActualEndField.Data.GetValue(0)) + GetFrameValue(trialActualEndField.Data.GetValue(1)) * 65535;

                // Some files have some values in BADC. Oh my fucking god.
                if (lastFrame - firstFrame + 1 > 0)
                {
                    return lastFrame - firstFrame + 1;
                }
                else
                {
                    throw new C3dBadFrameNumberFormatingException("");
                }
                    
            }
            catch (ParameterNotFoundException e)
            {
                // Do nothing and try the other parameters
            }
            catch (KeyNotFoundException e)
            { 
                // Do nothing and try the other parameters
            }
            catch (C3dBadFrameNumberFormatingException) { }

            // As per page 66 of the C3D User Guide
            return GetPointFrameValue("frames");
        }

        /// <summary>
        /// Extracts an integer value from a given object, handling multiple numeric types.
        /// </summary>
        /// <param name="value">The object to extract the integer value from. Can be an int, float, double, or null.</param>
        /// <returns>
        /// The integer value of the object if it is an int, float, or double.
        /// If the object is null or of an unsupported type, returns 0.
        /// </returns>
        internal int GetFrameValue(object? value)
        {
            return value switch
            {
                int i => i,
                float f => (int)f,
                double d => (int)d,
                _ => 0 // Default value if none of the above
            };
        }

        /// <summary>
        /// Retrieves the frame value from a specified POINT parameter in a C3D file.
        /// </summary>
        /// <param name="parameter">
        /// The name of the POINT parameter to retrieve the frame value from.
        /// Valid values are "frames" and "long_frames".
        /// </param>
        /// <returns>
        /// The integer frame value extracted from the specified parameter.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the provided parameter is not "frames" or "long_frames".
        /// </exception>
        /// <remarks>
        /// This method internally uses <see cref="GetFrameValue(object?)"/> to handle the conversion of the parameter value to an integer.
        /// </remarks>
        internal int GetPointFrameValue(string parameter)
        {
            object? frameNumber;
            switch (parameter.ToLower())
            {
                case "frames":
                    frameNumber = GetParameter("point", "frames").Data?.GetValue(0);
                    return GetFrameValue(frameNumber);

                case "long_frames":
                    frameNumber = GetParameter("point", "long_frames").Data?.GetValue(0);
                    return GetFrameValue(frameNumber);
                default:
                    throw new ArgumentException("Wrong POINT:XXXX for retriving Frames number.");
            }
        }


        /// <summary>
        /// Return the right amount of marker per frame.
        /// </summary>
        /// <param name="frameNumber">The number of frame in the C3D file.</param>
        /// <param name="analogChannels">The number of analog channels.</param>
        /// <param name="analogSamplePerFrame">The number of analog sample per Data Frame.</param>
        /// <param name="headerPointUsed">The value of POINT:USED present from the Header section.</param>
        /// <param name="parameterPointUsed">The value of POINT:USED present from the Parameter section.</param>
        /// <param name="pointerDataSection">The value the pointer to the Data section.</param>
        /// <param name="c3dStreamLength">The length in bytes of the C3D file.</param>
        /// <param name="dataTypeFile">The <see cref="DataType"/> of the C3D file.</param>
        /// <returns>The actual amount of marker per frame.</returns>
        /// <remarks>
        /// Some file have bad construction and features wrong value in either HEADER:POINT:USED or PARAMETER:POINT:USED. Reading them then comes down to luck.
        /// Despite the fact that the creator of those file messed up, they might still be usable.
        /// We assume the following values to be always truthful:
        /// <list type="bullet">
        ///     <item>PARAMETER:POINT:FRAME</item>
        ///     <item>PARAMETER:POINT:RATE</item>
        ///     <item>PARAMETER:ANALOG:RATE</item>
        /// </list>
        /// And that at least either the HEADER or PARAMETER value of POINT:USED is truthful.
        /// </remarks>
        /// <exception cref="C3dBadFormatingException">
        /// Thrown if no value of POINT:USED is valid for the amount of frame in the file.
        /// </exception>
        internal int GetRightAmountMarkerPerFrame(
            int frameNumber,
            int analogChannels,
            int analogSamplePerFrame,
            int headerPointUsed,
            int parameterPointUsed,
            int pointerDataSection,
            long c3dStreamLength,
            DataType dataTypeFile) 
        {
            long lengthFromHeader = frameNumber * (headerPointUsed * 4 + analogSamplePerFrame * analogChannels) * (int)dataTypeFile + pointerDataSection;
            long lengthFromParameter = frameNumber * (parameterPointUsed * 4 + analogSamplePerFrame * analogChannels) * (int)dataTypeFile + pointerDataSection;
            if (lengthFromHeader == lengthFromParameter || lengthFromHeader <= c3dStreamLength && lengthFromParameter > c3dStreamLength)
            {
                return headerPointUsed;
            }
            else if (lengthFromParameter <= c3dStreamLength && lengthFromHeader > c3dStreamLength)
            {
                return parameterPointUsed;
            }
            else
            {
                throw new C3dIncompatiblePointUsedValuesException("Incompatible values of HEADER:POINT:USED and PARAMETER:POINT:USED in regard to file length.");
            }
        }

        internal float ComputeScaleFactor()
        {
            float maxValue = 0f;
            foreach (C3dFileDataPoint[] points in Data.Points)
            {
                foreach (C3dFileDataPoint point in points)
                {
                    foreach (float dataPoint in point.Point)
                    {
                        if (dataPoint > maxValue)
                        {
                            maxValue = dataPoint;
                        }
                    }
                }
            }
            return maxValue / 32000;
        }

    }
}
