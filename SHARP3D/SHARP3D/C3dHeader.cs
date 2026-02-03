namespace SHARP3D
{
    ///<summary>
    /// Represents the header information of a C3D file format used in 3D motion capture data.
    /// It contains helper functions to parse and serialize the header data, as well as fields that map to the C3D header specifications.
    /// </summary>
    /// <para>
    /// The header are made of 512 bytes of information at the beginning of a C3D file.
    /// </para>
    public struct C3dHeader
    {
        ///<summary>
        /// A pointer to the first block of the parameter section.
        ///</summary>
        ///<remarks>
        /// Word: 1 (byte 1)
        ///</remarks>
        public byte PointerParameterSection;

        ///<summary>
        /// A flag defininig the Data section storage format which depends on the system used to acquire the data.
        ///</summary>
        ///<remarks>
        /// Word: 1 (byte 2)
        ///</remarks>
        public byte FlagDataFormat;

        ///<summary>
        /// The number of 3D points(markers) stored in each 3D frame.
        ///</summary>
        ///<remarks>
        /// Word: 2
        ///</remarks>
        public int MarkersPerFrame;

        ///<summary>
        /// The total number of analog channels stored in each 3D frame. If no analog data is stored, this value is zero.
        ///</summary>
        ///<remarks>
        /// Word: 3
        ///</remarks>
        public int AnalogSamplesPerFrame;

        ///<summary>
        /// The id number of the first frame of raw data transfered to the C3D file. This is not the id number of the first frame in the C3D file. This is the id number of the first frame from the raw data used to create the C3D file. It is not to be used and is here as a "just in case" according to the C3D documentation.
        ///</summary>
        ///<remarks>
        /// Word: 4
        ///</remarks>
        public int FirstFrameRawData;

        ///<summary>
        /// The id number of the last frame of raw data transfered to the C3D file. This is not the id number of the first frame in the C3D file. This is the id number of the first frame from the raw data used to create the C3D file. It is not to be used and is here as a "just in case" according to the C3D documentation.
        ///</summary>
        ///<remarks>
        /// Word: 5
        ///</remarks>
        public int LastFrameRawData;

        ///<summary>
        /// The maximum 3D frame interpolation gap present in the C3D file.
        ///</summary>
        ///<remarks>
        /// Word: 6
        ///</remarks>
        public int MaxFrameIntepolationGap;

        ///<summary>
        /// The floating-point factor that scales all 3D values into system measurement units. This transforms data stored as 16 bits signed integers to scale each of the stored 3D point and their residual values to floating point values, real world values. A positive scale value indicates that the data is stored as signed integers and a negative scale factor indicates that the data is stored as 32 bits floating point values. If the values are already in floating point, the scale factor doesn't need to be apllied to them. The Scale factor is computed by dividing the maximum absolute coordinate value by 32000.
        ///</summary>
        ///<remarks>
        /// Word: 7 - 8
        ///</remarks>
        public int ScaleFactor;

        ///<summary>
        /// A pointer to the first block of the data storage section.
        ///</summary>
        ///<remarks>
        /// Word: 9
        ///</remarks>
        public byte PointerDataSection;

        ///<summary>
        /// The analog sample rate per 3D frame.
        ///</summary>
        ///<remarks>
        /// Word: 10
        ///</remarks>
        public int AnalogSampleRatePerFrame;

        ///<summary>
        /// The 3D frame rate in hertz (frames per second).
        ///</summary>
        ///<remarks>
        /// Word: 11 - 12
        ///</remarks>
        public float Rate3dFrame;

        ///<summary>
        /// A key value indicating whether the C3D file supports 4-character event labels.
        ///</summary>
        ///<remarks>
        /// Word: 150
        ///</remarks>
        public bool Support4charEventLabels;

        ///<summary>
        /// Number of defined events in the C3D file.
        ///</summary>
        ///<remarks>
        /// Word: 151
        ///</remarks>
        public int DefinedEventsNb;

        ///<summary>
        /// Array of defined events in the C3D file. The events contain information such as event time, display flag, and event label.
        ///</summary>
        ///<remarks>
        /// Word: 153 - 188 (Event times in seconds, up to 18 events)<br/>
        /// Word: 189 - 197 (Event display flags 0x00=ON, 0x01=OFF)<br/>
        /// Word: 199 - 234 (Event labels, up to 4 characters each if Support4charEventLabels is true)
        ///</remarks>
        public C3dHeaderEvent[] Events;


        // TODO: Implement method to parse binaries into C3dHeader struct.
        ///<summary>
        ///
        ///</summary>
        public static C3dHeader FromBinaries(byte[] binaries)
        {
            return new C3dHeader();
        }

        // TODO: Implement method to convert C3dHeader struct into binaries.
        ///<summary>
        ///
        ///</summary>
        public static byte[] ToBinaries()
        {
            return new byte[0];
        }
    }
}
