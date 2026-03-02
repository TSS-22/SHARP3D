using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Header
{
    ///<summary>
    /// Represents the header information of a C3D file format used in 3D motion capture data.
    /// It contains helper functions to parse and serialize the header data, as well as fields that map to the C3D header specifications.
    /// </summary>
    /// <para>
    /// The header are made of 512 bytes of information at the beginning of a C3D file.
    /// </para>
    public struct C3dHeader : IEquatable<C3dHeader>
    {
        ///<summary>
        ///<para>
        /// Word: 1 (byte 1)
        ///</para>
        /// A pointer to the first block of the parameter section.
        ///</summary>
        public int PointerParameterSection;

        ///<summary>
        ///<para>
        /// Word: 1 (byte 2)
        ///</para>
        /// A flag defininig the data storage format which depends on the system and the software used to acquire the data.
        ///</summary>
        public StorageFormat StorageFormat;

        ///<summary>
        ///<para>
        /// Word: 2
        ///</para>
        /// The number of 3D points(markers) stored in each 3D frame.
        ///</summary>
        public int MarkersPerFrame;

        ///<summary>
        ///<para>
        /// Word: 3
        ///</para>
        /// The total number of analog channels stored in each 3D frame. If no analog data is stored, this value is zero.
        ///</summary>
        ///<remarks>
        ///This parameter encompass the total analog sample recorded and can be confusing. For example if you have a force plate with 6 channels recording at 4 times the 3D marker acquisition rate, then the value of that variable should be: 4 * 6 = 24. But even this doesn't match the value from the test files.
        /// </remarks>
        public int AnalogSamplesPerFrame;

        ///<summary>
        ///<para>
        /// Word: 4
        ///</para>
        /// The id number of the first frame of raw data transfered to the C3D file. This is not the id number of the first frame in the C3D file. This is the id number of the first frame from the raw data used to create the C3D file. It is not to be used and is here as a "just in case" according to the C3D documentation.
        ///</summary>
        public int FirstFrameRawData;

        ///<summary>
        ///<para>
        /// Word: 5
        ///</para>
        /// The id number of the last frame of raw data transfered to the C3D file. This is not the id number of the first frame in the C3D file. This is the id number of the first frame from the raw data used to create the C3D file. It is not to be used and is here as a "just in case" according to the C3D documentation.
        ///</summary>
        public int LastFrameRawData;

        ///<summary>
        ///<para>
        /// Word: 6
        ///</para>
        /// The maximum 3D frame interpolation gap present in the C3D file.
        ///</summary>
        public int MaxFrameIntepolationGap;

        ///<summary>
        ///<para>
        /// Word: 7 - 8
        ///</para>
        /// The floating-point factor that scales all 3D values into system measurement units. This transforms data stored as 16 bits signed integers to scale each of the stored 3D point and their residual values to floating point values, real world values. A positive scale value indicates that the data is stored as signed integers and a negative scale factor indicates that the data is stored as 32 bits floating point values. If the values are already in floating point, the scale factor doesn't need to be apllied to them. The Scale factor is computed by dividing the maximum absolute coordinate value by 32000.
        ///</summary>
        ///<remarks>
        ///The value given in the test suite 01 spreadsheet doesn't match the hex value from the file.
        /// </remarks>
        public float ScaleFactor;

        ///<summary>
        ///<para>
        /// Word: 9
        ///</para>
        /// A pointer to the first block of the data storage section. The pointer is the number of 512 bytes block.
        ///</summary>
        public int PointerDataSection;

        ///<summary>
        ///<para>
        /// Word: 10
        ///</para>
        /// The analog sample rate per 3D frame.
        ///</summary>
        public int AnalogSampleRatePerFrame;

        ///<summary>
        ///<para>
        /// Word: 11 - 12
        ///</para>
        /// The 3D frame rate in hertz (frames per second).
        ///</summary>
        public float Rate3dFrame;

        ///<summary>
        ///<para>
        /// Word: 150
        ///</para>
        /// A key value indicating whether the C3D file supports 4-character event labels. If value is 12345 (0x3039h), 4-character event labels are supported; otherwise, only 3-character labels are supported.
        ///</summary>
        public bool Support4charEventLabels;

        ///<summary>
        ///<para>
        /// Word: 151
        ///</para>
        /// Number of defined events in the C3D file.
        ///</summary>
        public int EventsNb;

        ///<summary>
        ///<para>
        /// Word: 153 - 188 (Event times in seconds, up to 18 events)<br/>
        /// Word: 189 - 197 (Event display flags 0x00=ON, 0x01=OFF)<br/>
        /// Word: 199 - 234 (Event labels, up to 4 characters each if Support4charEventLabels is true)
        ///</para>
        /// Array of defined events in the C3D file. The events contain information such as event time, display flag, and event label.
        ///</summary>
        public C3dHeaderEvent[] Events;


        public bool Equals(C3dHeader other)
        {
            return PointerParameterSection == other.PointerParameterSection &&
                   StorageFormat == other.StorageFormat &&
                   MarkersPerFrame == other.MarkersPerFrame &&
                   AnalogSamplesPerFrame == other.AnalogSamplesPerFrame &&
                   FirstFrameRawData == other.FirstFrameRawData &&
                   LastFrameRawData == other.LastFrameRawData &&
                   MaxFrameIntepolationGap == other.MaxFrameIntepolationGap &&
                   ScaleFactor == other.ScaleFactor &&
                   PointerDataSection == other.PointerDataSection &&
                   AnalogSampleRatePerFrame == other.AnalogSampleRatePerFrame &&
                   Rate3dFrame == other.Rate3dFrame &&
                   Support4charEventLabels == other.Support4charEventLabels &&
                   EventsNb == other.EventsNb &&
                   ((Events == null && other.Events == null) ||
                    (Events != null && other.Events != null &&
                     Events.Length == other.Events.Length &&
                     !Events.Where((t, i) => !t.Equals(other.Events[i])).Any()));
        }

        public override bool Equals(object obj)
        {
            return obj is C3dHeader other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + PointerParameterSection.GetHashCode();
                hash = hash * 23 + StorageFormat.GetHashCode();
                hash = hash * 23 + MarkersPerFrame.GetHashCode();
                hash = hash * 23 + AnalogSamplesPerFrame.GetHashCode();
                hash = hash * 23 + FirstFrameRawData.GetHashCode();
                hash = hash * 23 + LastFrameRawData.GetHashCode();
                hash = hash * 23 + MaxFrameIntepolationGap.GetHashCode();
                hash = hash * 23 + ScaleFactor.GetHashCode();
                hash = hash * 23 + PointerDataSection.GetHashCode();
                hash = hash * 23 + AnalogSampleRatePerFrame.GetHashCode();
                hash = hash * 23 + Rate3dFrame.GetHashCode();
                hash = hash * 23 + Support4charEventLabels.GetHashCode();
                hash = hash * 23 + EventsNb.GetHashCode();

                if (Events != null)
                {
                    foreach (var ev in Events)
                        hash = hash * 23 + (ev?.GetHashCode() ?? 0);
                }

                return hash;
            }
        }

        public static bool operator ==(C3dHeader left, C3dHeader right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(C3dHeader left, C3dHeader right)
        {
            return !left.Equals(right);
        }

        // TODO: Try to "reverse compute" the scale factor. Indeed if it is just found by dividing the max absolute value by 32000, mathematically I can find it back with the max value read by the int16 value.
        // TODO: Implement method to parse binaries into C3dHeader struct.
        public static C3dHeader FromBinaries(byte[] binaries, ProcessorType processorFile)
        {
            byte[] pointerParameterSectionBinaries = { 0, binaries[0] };

            return new C3dHeader
            {
                PointerParameterSection = BitConverter.ToInt16(pointerParameterSectionBinaries, 0),
                StorageFormat = Convert.ToChar(binaries[1]) == 'P' ? StorageFormat.ORIGINAL : StorageFormat.UNKOWN,
                MarkersPerFrame = C3dBytesConvertor.ToInt(binaries.Skip(2).Take(2).ToArray(), processorFile),
                AnalogSamplesPerFrame = C3dBytesConvertor.ToInt(binaries.Skip(4).Take(2).ToArray(), processorFile),
                FirstFrameRawData = C3dBytesConvertor.ToInt(binaries.Skip(6).Take(2).ToArray(), processorFile),
                LastFrameRawData = C3dBytesConvertor.ToInt(binaries.Skip(8).Take(2).ToArray(), processorFile),
                MaxFrameIntepolationGap = C3dBytesConvertor.ToInt(binaries.Skip(10).Take(2).ToArray(), processorFile),
                ScaleFactor = Math.Abs(C3dBytesConvertor.ToFloat(binaries.Skip(12).Take(4).ToArray(), processorFile)),
                PointerDataSection = C3dBytesConvertor.ToInt(binaries.Skip(16).Take(2).ToArray(), processorFile),
                AnalogSampleRatePerFrame = C3dBytesConvertor.ToInt(binaries.Skip(18).Take(2).ToArray(), processorFile),
                Rate3dFrame = C3dBytesConvertor.ToFloat(binaries.Skip(20).Take(4).ToArray(), processorFile),
                Support4charEventLabels = C3dBytesConvertor.ToInt(binaries.Skip(298).Take(2).ToArray(), processorFile) == 12345 ? true : false,
                EventsNb = C3dBytesConvertor.ToInt(binaries.Skip(300).Take(2).ToArray(), processorFile),
                Events = C3dHeaderEvent.EventsFromBinaries(
                    binaries.Skip(304).Take(208).ToArray(), // Event binaries
                    C3dBytesConvertor.ToInt(binaries.Skip(300).Take(2).ToArray(), processorFile), // Nb of events
                    C3dBytesConvertor.ToInt(binaries.Skip(298).Take(2).ToArray(), processorFile) == 12345 ? true : false, // Support 4 char event labels
                    processorFile
                    ), // Argument will need to be fixed
            };
        }

    }

    
}
