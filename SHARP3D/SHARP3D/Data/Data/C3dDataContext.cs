using SHARP3D.Utils.Enum;

namespace SHARP3D.Data.Data
{
    // TODO: Check that all the values are actually used and therefore necessary.

    /// <summary>
    /// Represents the context for reading and processing data from a C3D file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class encapsulates all necessary information and parameters required to read and interpret
    /// the data section of a C3D file, including file stream, processor type, data type, and scaling factors.
    /// </para>
    /// </remarks>
    public class C3dDataContext
    {
        /// <summary>
        /// Gets the file stream used to access the C3D file.
        /// </summary>
        public FileStream C3dStream { get; }

        /// <summary>
        /// Gets the processor type used to create the C3D file.
        /// </summary>
        public ProcessorType Processor { get; }

        /// <summary>
        /// Gets the data type of the values stored in the C3D file.
        /// </summary>
        public DataType DataTypeFile { get; }

        /// <summary>
        /// Gets the pointer to the start of the data section in the C3D file.
        /// </summary>
        public int PointerDataSection { get; }

        /// <summary>
        /// Gets the total number of frames in the C3D file.
        /// </summary>
        public int FramesNumber { get; }

        /// <summary>
        /// Gets the number of markers per frame in the C3D file.
        /// </summary>
        public int MarkersPerFrame { get; }

        /// <summary>
        /// Gets the acquisition rate of the 3D point data, in Hz.
        /// </summary>
        public float PointRate { get; }

        /// <summary>
        /// Gets the acquisition rate of the analog data, in Hz.
        /// </summary>
        public float AnalogRate { get; }

        /// <summary>
        /// Gets the number of analog channels in the C3D file.
        /// </summary>
        public int AnalogChannels { get; }

        /// <summary>
        /// Gets the scale factor applied to 3D point coordinates.
        /// </summary>
        public float PointScaleFactor { get; }

        /// <summary>
        /// Gets the general scale factor applied to all analog data.
        /// </summary>
        public float AnalogGeneralScaleFactor { get; }

        /// <summary>
        /// Gets the scale factors applied to individual analog channels.
        /// </summary>
        /// <remarks>
        /// Each element in the array corresponds to the scale factor for a specific analog channel.
        /// </remarks>
        public float[] AnalogChannelScaleFactor { get; }

        /// <summary>
        /// Gets the number of analog samples per 3D frame.
        /// </summary>
        /// <remarks>
        /// This value is calculated as the ratio of <see cref="AnalogRate"/> to <see cref="PointRate"/>.
        /// </remarks>
        public int AnalogSamplePerFrame { get; }

        /// <summary>
        /// Gets the offset for analog data in the C3D file.
        /// </summary>
        public int[] AnalogOffset {  get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="C3dDataContext"/> class.
        /// </summary>
        /// <param name="c3dStream">The file stream used to access the C3D file.</param>
        /// <param name="processor">The processor type used to create the C3D file.</param>
        /// <param name="dataTypeFile">The data type of the values stored in the C3D file.</param>
        /// <param name="pointerDataSection">The pointer to the start of the data section in the C3D file.</param>
        /// <param name="framesNumber">The total number of frames in the C3D file.</param>
        /// <param name="markersPerFrame">The number of markers per frame in the C3D file.</param>
        /// <param name="pointRate">The acquisition rate of the 3D point data, in Hz.</param>
        /// <param name="analogRate">The acquisition rate of the analog data, in Hz.</param>
        /// <param name="analogChannels">The number of analog channels in the C3D file.</param>
        /// <param name="pointScaleFactor">The scale factor applied to 3D point coordinates.</param>
        /// <summary>
        /// <param name="analogGeneralScaleFactor">The general scale factor applied to all analog data.</param>
        /// </summary>
        /// <param name="analogChannelScaleFactor">The scale factors applied to individual analog channels.</param>
        /// <param name="analogOffset">The offset for analog data in the C3D file.</param>
        /// <param name="analogSamplePerFrame">The number of analog sample in each frame.</param>
        /// <exception cref="Exception">
        /// Thrown if the ratio of <paramref name="analogRate"/> to <paramref name="pointRate"/> is not an integer.
        /// </exception>
        public C3dDataContext(
            FileStream c3dStream,
            ProcessorType processor,
            DataType dataTypeFile,
            int pointerDataSection,
            int framesNumber,
            int markersPerFrame,
            float pointRate,
            float analogRate,
            int analogChannels,
            float pointScaleFactor,
            float analogGeneralScaleFactor,
            float []analogChannelScaleFactor,
            int[] analogOffset,
            int analogSamplePerFrame)
        {
            C3dStream = c3dStream;
            Processor = processor;
            DataTypeFile = dataTypeFile;
            PointerDataSection = pointerDataSection;
            FramesNumber = framesNumber;
            MarkersPerFrame = markersPerFrame;
            PointRate = pointRate;
            AnalogRate = analogRate;
            AnalogChannels = analogChannels;
            AnalogSamplePerFrame = analogSamplePerFrame;
            PointScaleFactor = pointScaleFactor;
            AnalogGeneralScaleFactor = analogGeneralScaleFactor;
            AnalogChannelScaleFactor = analogChannelScaleFactor;
            AnalogOffset = analogOffset;
        }
    }
}
