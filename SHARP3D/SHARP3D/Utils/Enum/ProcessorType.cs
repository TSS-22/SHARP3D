namespace SHARP3D.Utils.Enum
{
    /// <summary>
    /// <para>This structure index the different processor types used to create C3D files.</para>
    /// </summary>
    public enum ProcessorType : byte
    {
        /// <summary>
        /// Represents the INTEL vendor with an associated value of 84.
        /// </summary>
        INTEL = 84,
        /// <summary>
        /// Represents the DEC opcode with a value of 85.
        /// </summary>
        DEC = 85,
        /// <summary>
        /// Specifies the signature type for MIPS architecture.
        /// </summary>
        SIG_MIPS = 86,
        /// <summary>
        /// Represents an unknown value with a numeric value of 255.
        /// </summary>
        UNKOWN = 255,
    }

    /// <summary>
    /// Helper/conversion methods for <see cref="ProcessorType"/>.
    /// </summary>
    public static class ProcessorTypeExtensions
    {
        /// <summary>
        /// Converts a <see cref="ProcessorType"/> to its underlying <see cref="byte"/> value.
        /// </summary>
        public static byte ToByte(this ProcessorType processorType) => (byte)processorType;

        /// <summary>
        /// Converts an <see cref="int"/> to <see cref="ProcessorType"/>; if value is not linked to a recognized processor type, it is affected to <see cref="ProcessorType.UNKOWN"/>.
        /// </summary>
        public static ProcessorType FromInt(int value)
        {
            switch (value)
            {
                case 84: return ProcessorType.INTEL;
                case 85: return ProcessorType.DEC;
                case 86: return ProcessorType.SIG_MIPS;
                default: return ProcessorType.UNKOWN;
            }
        }

        /// <summary>
        /// Converts a <see cref="byte"/> to <see cref="ProcessorType"/>; If CPU architecture is not linked to a recognized processor type, it is affected to <see cref="ProcessorType.UNKOWN"/>.
        /// </summary>
        /// <param name="value">The byte value to convert.</param>
        /// <returns>A <see cref="ProcessorType"/> corresponding to the provided byte value, or <see cref="ProcessorType.UNKOWN"/> if the value does not match any known processor type.</returns>
        public static ProcessorType FromByte(byte value)
        {
            switch (value)
            {
                case 84: return ProcessorType.INTEL;
                case 85: return ProcessorType.DEC;
                case 86: return ProcessorType.SIG_MIPS;
                default: return ProcessorType.UNKOWN;
            }
        }

        /// <summary>
        /// Converts a ProcessorType value to its underlying integer representation.
        /// </summary>
        /// <param name="processorType">The ProcessorType value to convert.</param>
        /// <returns>The integer representation of the specified ProcessorType.</returns>
        public static int ToInt(this ProcessorType processorType) => (int)processorType;
    }
}