csharp SHARP3D\Extensions\ProcessorTypeExtensions.cs
using System;

namespace SHARP3D
{
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
        /// Attempts to convert an <see cref="int"/> to a <see cref="ProcessorType"/>.
        /// Returns true when the value maps to a defined enum member.
        /// </summary>
        public static bool TryFromInt(int value, out ProcessorType processorType)
        {
            var b = (byte)value;
            if (Enum.IsDefined(typeof(ProcessorType), b))
            {
                processorType = (ProcessorType)b;
                return true;
            }

            processorType = default;
            return false;
        }

        /// <summary>
        /// Converts an <see cref="int"/> to <see cref="ProcessorType"/>; throws when the value is not defined.
        /// </summary>
        public static ProcessorType FromInt(int value)
        {
            if (TryFromInt(value, out var pt))
            {
                return pt;
            }
            else {
                return ProcessorType.UNKOWN;
            }
        }

        /// <summary>
        /// Attempts to convert a <see cref="byte"/> to a <see cref="ProcessorType"/>.
        /// </summary>
        public static bool TryFromByte(byte value, out ProcessorType processorType)
        {
            if (Enum.IsDefined(typeof(ProcessorType), value))
            {
                processorType = (ProcessorType)value;
                return true;
            }

            processorType = default;
            return false;
        }

        /// <summary>
        /// Converts a <see cref="byte"/> to <see cref="ProcessorType"/>; throws when the value is not defined.
        /// </summary>
        public static ProcessorType FromByte(byte value)
        {
            if (TryFromByte(value, out var pt)) return pt;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Value is not a valid ProcessorType.");
        }
    }
}