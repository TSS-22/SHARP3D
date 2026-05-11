using SHARP3D.Data;
using SHARP3D.Data.Data;
using SHARP3D.Exceptions;
using SHARP3D.Header;
using SHARP3D.Parameter;
using SHARP3D.Parameter.Data;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
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
        /// Represents the file stream used to access the C3D file.
        /// </summary>
        public FileStream? C3dStream { get; set; } = null;
        
        /// <summary>
        /// Specifies the type of processor that was used to create the C3D file.
        /// </summary>
        public ProcessorType ProcessorFile { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;

        /// <summary>
        /// Gets or sets the type of processor used by the host system.
        /// </summary>
        public ProcessorType ProcessorHost { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;

        /// <summary>
        /// Gets or sets the pointer to the parameter section in the C3D file.
        /// </summary>
        public int PointerParameterSection { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the data section in the C3D file.
        /// </summary>
        public int PointerDataSection { get; set; }

        /// <summary>
        /// Gets or sets the scale factor used for 3D point data in the C3D file.
        /// </summary>
        public float ScaleFactor { get; set; }

        /// <summary>
        /// Gets or sets the data type used in the C3D file.
        /// </summary>
        public DataType DataTypeFile { get; set; }

        /// <summary>
        /// Gets or sets the header information of the C3D file.
        /// </summary>
        public C3dHeader Header { get; set; } = new C3dHeader();

        /// <summary>
        /// Gets or sets the list of parameter groups in the C3D file.
        /// </summary>
        public List<C3dParameterGroup> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the collection of parameters in the C3D file.
        /// </summary>
        public C3dParameterCollection ParameterCollection { get; set; }

        /// <summary>
        /// Centralize the values needed to extract the data from the C3D file.
        /// </summary>
        /// <remarks>
        /// It is saved as a Class field for testing, and to help work around bad formatting from files at the moment. It might be discarded later or at least rearranged.
        /// </remarks>
        public C3dDataContext DataContext { get; set; }

        /// <summary>
        /// Gets or sets the data contained in the C3D file.
        /// </summary>
        public C3dData Data { get; set; }

        

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
            C3dStream = fileStream;
            ProcessorFile = ReadProcessorByte(fileStream); 
            float tempScaleFactor = GetPointScaleFactor(fileStream, ProcessorFile);
            ScaleFactor = Math.Abs(tempScaleFactor);
            DataTypeFile = tempScaleFactor < 0 ? DataType.FLOAT32 : DataType.INT16;
            
            Header = GetHeader(C3dStream, ProcessorFile);

            PointerParameterSection = Header.PointerParameterSection;
            PointerDataSection = Header.PointerDataSection;

            Parameters = GetParameters(C3dStream, ProcessorFile, PointerParameterSection, PointerDataSection);

            ParameterCollection = new C3dParameterCollection(Parameters);

            Data = GetData(C3dStream, ProcessorFile, DataTypeFile, ScaleFactor);

            CloseFileStream();
        }

        /// <summary>
        /// Creates an empty instance of the <see cref="C3dFile"/> class.
        /// </summary>
        /// <returns>An empty <see cref="C3dFile"/> instance.</returns>
        public C3dFile CreateEmpty()
        {
            return new C3dFile();
        }

        /// <summary>
        /// Reads the header information from the C3D file.
        /// </summary>
        /// <param name="fileStream">The file stream to read from.</param>
        /// <param name="processorFile">The processor type used to create the C3D file.</param>
        /// <returns>A <see cref="C3dHeader"/> object containing the header information.</returns>
        internal C3dHeader GetHeader(FileStream fileStream, ProcessorType processorFile)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] headerBinaries = ReadHeaderBinaries(fileStream);
            return C3dHeader.FromBinaries(headerBinaries, processorFile);
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
        internal List<C3dParameterGroup> GetParameters(FileStream fileStream, ProcessorType processorFile, int pointerParameterSection, int pointerDataSection)
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
            return ((C3dBytesConvertor.ToInt(pointerToData, processor) - 1) * 512); // I don't know why you have to substrack 1.
        }

        /// <summary>
        /// Gets the number of parameter blocks in the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <returns>The number of parameter blocks.</returns>
        internal static int GetParameterBlockCount(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 2, SeekOrigin.Begin);
            return c3dStream.ReadByte();
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
        internal float GetPointScaleFactor(FileStream c3dStream, ProcessorType processor)
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
        /// Reads the parameter binaries from the C3D file.
        /// </summary>
        /// <param name="c3dStream">The file stream to read from.</param>
        /// <param name="parameterSectionPointer">The pointer to the parameter section.</param>
        /// <param name="parameterBlockCount">The number of parameter blocks.</param>
        /// <returns>A byte array containing the parameter binaries.</returns>
        internal static byte[] ReadParameterBinaries(FileStream c3dStream, int parameterSectionPointer, int parameterBlockCount)
        {
            byte[] parameters = new byte[parameterBlockCount * 512];
            c3dStream.Seek(parameterSectionPointer, SeekOrigin.Begin);
            c3dStream.ReadExactly(parameters, 0, parameterBlockCount * 512);
            return parameters;
        }

        /// <summary>
        /// Gets a parameter from the C3D file by its group and parameter name.
        /// </summary>
        /// <param name="groupName">The name of the parameter group.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>A <see cref="C3dParameter"/> object representing the requested parameter.</returns>
        public C3dParameter GetParameter(string groupName, string parameterName)
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
        /// <param name="pointScaleFactor">The point scale factor.</param>
        /// <returns>A <see cref="C3dData"/> object containing the data.</returns>
        internal C3dData GetData(FileStream c3dStream, ProcessorType processor, DataType dataTypeFile, float pointScaleFactor)
        {
            int pointerDataSection = GetDataSectionPointer(c3dStream, processor);
            int framesNumber = GetRightAmountOfFrames();
            float pointRate = (GetParameter("point", "rate").Data?.GetValue(0) as float?) ?? 0f;
            int markersPerFrame = (GetParameter("point", "used").Data?.GetValue(0) as int?) ?? 0;

            // Some application don't give a fuck about the ANALOG mandatory parameters
            // NaturalPoint as per Sample29 readme might be one one of those. Optitrack also.
            float analogRate = 0;
            try 
            { 
                analogRate = (GetParameter("analog", "rate").Data?.GetValue(0) as float?) ?? 0f; // Contradiction in the C3D documentation
            }
            catch (ParameterNotFoundException ex) { }

            int analogChannels = 0;
            try { analogChannels = (GetParameter("analog", "used").Data?.GetValue(0) as int?) ?? 0; } catch (ParameterNotFoundException ex) { }

            float analogGeneralScaleFactor = 0.0f;
            try { analogGeneralScaleFactor = (GetParameter("analog", "gen_scale").Data?.GetValue(0) as float?) ?? 0f; } catch (ParameterNotFoundException ex) { }

            float[] tempAnalogChannelScaleFactor = new float[] { 0f };
            try { tempAnalogChannelScaleFactor = (GetParameter("analog", "scale").Data as float[]) ?? new float[] { 0f }; } catch (ParameterNotFoundException ex) { }

            float[] analogChannelScaleFactor;
            if (tempAnalogChannelScaleFactor.Length >= analogChannels) 
            {
                analogChannelScaleFactor = tempAnalogChannelScaleFactor.Take(analogChannels).ToArray();
            }
            else // Some files don't have enough ANALOG:SCALE_CHANNEL. They seems to only have 1 as the scale factor, hence we just add 1 for the missing indexes.
            {
                
                float[] paddedArray = new float[analogChannels];

                // Copy the original values
                Array.Copy(tempAnalogChannelScaleFactor, paddedArray, tempAnalogChannelScaleFactor.Length);

                // Fill the remaining positions with 1
                for (int i = tempAnalogChannelScaleFactor.Length; i < analogChannels; i++)
                {
                    paddedArray[i] = 1f;
                }
                analogChannelScaleFactor = paddedArray;
            }


                // Some software have the analogoff set as a float.
                //int analogOffset = 0;
                int[] analogOffset = new int[analogChannels];
            try
            {
                analogOffset = GetParameter("analog", "offset").Data?
                    .OfType<object>()
                    .Select(obj => Convert.ToInt32(obj))
                    .ToArray() ?? Array.Empty<int>();

            }
            catch (IndexOutOfRangeException) { }
            catch (ParameterNotFoundException ex) { }

            // That's the default so we don't care if it is anything else than unsigned. If we can find better strategy than the one from C3D User guide, we will implement it here.
            AnalogFormatFlag analogFormat = AnalogFormatFlag.SIGNED;

            Array analogFormatValue = Sharp3dConstants.SignedArrayString;
            try
            {
                analogFormatValue = GetParameter("analog", "format").Data;
            }
            catch(ParameterNotFoundException ex)
            {
                Console.WriteLine("No ANALOG:FORMAT parameter found. Defaulting to SIGNED format for analog data.");
            }

            if ( analogFormatValue == Sharp3dConstants.UnsignedArrayString)
            {
                analogFormat = AnalogFormatFlag.UNSIGNED;
            }

            // TODO: actually sort the error that can come
            DataContext = new C3dDataContext(
                c3dStream: c3dStream,
                processor: processor,
                dataTypeFile: dataTypeFile,
                pointerDataSection: pointerDataSection,
                framesNumber: framesNumber,
                markersPerFrame: GetRightAmountMarkerPerFrame(
                framesNumber,
                analogChannels,
                GetAnalogSamplePerFrame(pointRate, analogRate),
                    Header.MarkersPerFrame,
                    markersPerFrame,
                    pointerDataSection,
                    c3dStream.Length,
                    dataTypeFile
                    ),
                pointRate: pointRate,
                analogRate: analogRate,
                analogChannels:analogChannels,
                pointScaleFactor: pointScaleFactor,
                analogGeneralScaleFactor: analogGeneralScaleFactor,
                analogChannelScaleFactor: analogChannelScaleFactor,
                analogOffset: analogOffset,
                analogSamplePerFrame: GetAnalogSamplePerFrame(pointRate, analogRate),
                analogFormat: analogFormat
                );
            
            

            return C3dDataHelper.FromFileStream(DataContext);
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
                return Header.AnalogSampleRatePerFrame;
            }
            else
            {
                return ((int)analogSamplePerFrame);
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
                C3dParameter trialActualStartField = GetParameter("trial", "actual_start_field");
                C3dParameter trialActualEndField = GetParameter("trial", "actual_end_field");

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
        /// We assume the followin values to be always truthful:
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
            long lengthFromHeader = frameNumber * ((headerPointUsed * 4 + analogSamplePerFrame * analogChannels) * (int)dataTypeFile) + pointerDataSection;
            long lengthFromParameter = frameNumber * ((parameterPointUsed * 4 + analogSamplePerFrame * analogChannels) * (int)dataTypeFile) + pointerDataSection;
            if ((lengthFromHeader == lengthFromParameter) || ((lengthFromHeader <= c3dStreamLength) && (lengthFromParameter > c3dStreamLength)))
            {
                return headerPointUsed;
            }
            else if ((lengthFromParameter <= c3dStreamLength) && (lengthFromHeader > c3dStreamLength))
            {
                return parameterPointUsed;
            }
            else
            {
                throw new C3dIncompatiblePointUsedValuesException("Incompatible values of HEADER:POINT:USED and PARAMETER:POINT:USED in regard to file length.");
            }
        }

        /// <summary>
        /// Converts the C3D file to a byte array.
        /// </summary>
        /// <returns>A byte array representing the C3D file.</returns>
        /// <remarks>
        /// TODO: Implement actual binaries transformation logic.
        /// </remarks>
        public byte[] ToBinaries()
        {
            return new byte[] { };
        }

        /// <summary>
        /// Saves the C3D file to the specified file path.
        /// </summary>
        /// <param name="filepath">The path to save the C3D file to.</param>
        /// <returns>An integer representing the result of the save operation (0 for success).</returns>
        /// <remarks>
        /// TODO: Implement actual file saving logic. Return 0 if success, else error code. Use the C3dFileManager for this.
        /// </remarks>
        public int SaveToFile(string filepath)
        {
            return 0;
        }

        /// <summary>
        /// Checks if the file stream is open.
        /// </summary>
        /// <returns>True if the file stream is open; otherwise, false.</returns>
        public bool IsFileStreamOpen()
        {
            return C3dStream != null;
        }

        /// <summary>
        /// Closes the file stream.
        /// </summary>
        public void CloseFileStream()
        {
            if (C3dStream != null)
            {
                C3dStream.Close();
                C3dStream = null;
            }
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
    }
}
