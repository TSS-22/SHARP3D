using System.Security.Cryptography.X509Certificates;

namespace SHARP3D
{
    /// <summary>
    /// <para>This structure index the different processor types used to create C3D files.</para>
    /// </summary>
    public enum ProcessorType : byte
    {
        INTEL = 84,
        DEC = 85,
        SIG_MIPS = 86,
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
        /// Converts a <see cref="byte"/> to <see cref="ProcessorType"/>; throws when the value is not defined.
        /// </summary>
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

        public static byte ToByte(this int value) => (byte)value;

        public static int ToInt(this ProcessorType processorType) => (int)processorType;
    }
}