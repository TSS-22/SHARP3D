using SHARP3D.Exceptions;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Utils
{

    // TODO: Handle unknown processor type. I think I need to handle it higher up the chain and refuse unkown processor type C3D files.
    // TODO: Add the check about number of bytes and such.

    /// <summary>
    /// This helper class provide functions to process C3D bytes according to the file maker processor type for compatibility purposes.
    /// </summary>
    /// <para>
    /// The Intel floating point format is little-endian IEEE 754.
    /// </para>
    /// <remarks>
    /// The functions:
    /// <list type="bullet">
    ///   <item><see cref ="LittleEndianFloatToVaxFloatByte"/></item>
    ///   <item><see cref ="BigEndianFloatToVaxF"/></item>
    ///   <item><see cref ="VaxFToLittleEndianFloat"/></item>
    ///   <item><see cref ="VaxFToBigEndianFloat"/></item>
    /// </list>
    /// are modifications of the code from [njuffa](https://stackoverflow.com/users/780717/njuffa) at the quesiton [Floating Point numbers on VAX machine](https://stackoverflow.com/questions/71689829/floating-point-numbers-on-vax-machine).
    /// They are therefore licensed under the [Creative Commons Attribution-ShareAlike 4.0 International License](https://creativecommons.org/licenses/by-sa/4.0/). This project is nonetheless not endorsed by the original author and the code is used here under the terms of the license.
    /// </remarks>
    public static class C3dBytesConvertor
    {

        private const float TWO_TO_M128 = 2.93873588e-39f; // 2**(-128)
        private const float TWO_TO_127 = 1.70141184e+38f; // 2**127
        private const float TWO_TO_126 = 8.50705917e+37f; // 2**126
        private const float SCAL = 4f; // factor between IEEE-754 'binary32' and VAX F-float

        /// <summary>
        /// Converts a 16-bit integer to a byte array with byte order determined by the specified processor type.
        /// </summary>
        /// <param name="value">The 16-bit integer to convert.</param>
        /// <param name="processorMakerType">The processor type that determines the byte order.</param>
        /// <returns>A byte array representing the integer in the appropriate byte order.</returns>
        /// <exception cref="UnknownProcessorTypeException">Thrown when the specified processor type is not recognized.</exception>
        public static byte[] ToBytes(int value, ProcessorType processor)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (processor == ProcessorType.INTEL || processor == ProcessorType.DEC)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return bytes.Take(2).ToArray();
                }
                else if (processor == ProcessorType.SIG_MIPS)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    Array.Reverse(bytes);
                    return bytes.Skip(2).Take(2).ToArray();
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert Int to bytes.");
                }
            }
            // TODO: I need to test this.
            else
            {
                if (processor == ProcessorType.INTEL || processor == ProcessorType.SIG_MIPS)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    Array.Reverse(bytes);
                    return bytes.Skip(2).Take(2).ToArray();
                }
                else if (processor == ProcessorType.DEC)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return bytes.Take(2).ToArray();
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert Int to bytes.");
                }
            }
        }

        /// <summary>
        /// Converts a single-precision (32-bit) floating-point value to a byte array formatted for the specified processor type.
        /// </summary>
        /// <param name="value">The floating-point value to convert.</param>
        /// <param name="processorMakerType">The target processor type determining the byte order and format.</param>
        /// <returns>A byte array representing the floating-point value in the format required by the specified processor type.</returns>
        /// <exception cref="UnknownProcessorTypeException">Thrown when the specified processor type is not supported.</exception>
        public static byte[] ToBytes(float value, ProcessorType processor)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (processor == ProcessorType.INTEL)
                {
                    return BitConverter.GetBytes(value);
                }
                else if (processor == ProcessorType.DEC)
                {
                    return LittleEndianFloatToVaxFloatByte(value);
                }
                else if (processor == ProcessorType.SIG_MIPS)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    Array.Reverse(bytes);
                    return bytes;
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert Float to bytes.");
                }
            }
            else
            {
                if (processor == ProcessorType.INTEL)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    Array.Reverse(bytes);
                    return bytes;
                }
                else if (processor == ProcessorType.DEC)
                {
                    return BigEndianFloatToVaxF(value);
                }
                else if (processor == ProcessorType.SIG_MIPS)
                {
                    return BitConverter.GetBytes(value);
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert Float to bytes.");
                }
            }
        }

        /// <summary>
        /// Converts a byte array to a 16-bit integer using the specified processor type for endianness handling.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <param name="processorMakerType">The processor type indicating the expected byte order.</param>
        /// <returns>The 32-bit integer representation of the byte array.</returns>
        /// <exception cref="UnknownProcessorTypeException">Thrown when the processor type is unknown or unsupported.</exception>
        public static int ToInt(byte[] bytes, ProcessorType processor)
        {
            if (bytes == null) { throw new ArgumentNullException("bytes"); }
            if (bytes.Length != 2) { throw new ArgumentException("Bytes array must have a length of 2.", "bytes"); }
            if (BitConverter.IsLittleEndian)
            {
                if (processor == ProcessorType.INTEL || processor == ProcessorType.DEC)
                {
                    return BitConverter.ToInt16(bytes, 0);
                }
                else if (processor == ProcessorType.SIG_MIPS)
                {
                    Array.Reverse(bytes);
                    return BitConverter.ToInt16(bytes, 0);
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert bytes to Int.");
                }
            }
            else
            {
                if (processor == ProcessorType.INTEL || processor == ProcessorType.SIG_MIPS)
                {
                    Array.Reverse(bytes);
                    return BitConverter.ToInt16(bytes, 0);
                }
                else if (processor == ProcessorType.DEC)
                {
                    return BitConverter.ToInt16(bytes, 0);
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert bytes to Int.");
                }
            }
        }

        public static int ToUInt(byte[] bytes, ProcessorType processor)
        {
            if (bytes == null) { throw new ArgumentNullException("bytes"); }
            if (bytes.Length != 2) { throw new ArgumentException("Bytes array must have a length of 2.", "bytes"); }
            if (BitConverter.IsLittleEndian)
            {
                if (processor == ProcessorType.INTEL || processor == ProcessorType.DEC)
                {
                    return BitConverter.ToUInt16(bytes, 0);
                }
                else if (processor == ProcessorType.SIG_MIPS)
                {
                    Array.Reverse(bytes);
                    return BitConverter.ToUInt16(bytes, 0);
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert bytes to Int.");
                }
            }
            else
            {
                if (processor == ProcessorType.INTEL || processor == ProcessorType.SIG_MIPS)
                {
                    Array.Reverse(bytes);
                    return BitConverter.ToUInt16(bytes, 0);
                }
                else if (processor == ProcessorType.DEC)
                {
                    return BitConverter.ToUInt16(bytes, 0);
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert bytes to Int.");
                }
            }
        }

        /// <summary>
        /// Converts a byte array to a single-precision floating-point value based on the specified processor type and
        /// system endianness.
        /// </summary>
        /// <param name="bytes">The byte array representing the floating-point value.</param>
        /// <param name="processorMakerType">The processor type indicating the format of the floating-point value.</param>
        /// <returns>A single-precision floating-point value converted from the byte array.</returns>
        /// <exception cref="UnknownProcessorTypeException">Thrown when the processor type is unknown or unsupported.</exception>
        public static float ToFloat(byte[] bytes, ProcessorType processor)
        {
            if (bytes == null) { throw new ArgumentNullException("bytes"); }
            if (bytes.Length != 4) { throw new ArgumentException("Bytes array must have a length of 4.", "bytes"); }
            if (BitConverter.IsLittleEndian)
            {
                if (processor == ProcessorType.INTEL)
                {
                    return BitConverter.ToSingle(bytes, 0);
                }
                else if (processor == ProcessorType.DEC)
                {
                    return VaxFToLittleEndianFloat(bytes);
                }
                else if (processor == ProcessorType.SIG_MIPS)
                {
                    Array.Reverse(bytes);
                    return BitConverter.ToSingle(bytes, 0);
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert bytes to Float.");
                }
            }
            else
            {
                if (processor == ProcessorType.INTEL)
                {
                    Array.Reverse(bytes);
                    return BitConverter.ToSingle(bytes, 0);
                }
                else if (processor == ProcessorType.DEC)
                {
                    return VaxFToBigEndianFloat(bytes);
                }
                else if (processor == ProcessorType.SIG_MIPS)
                {
                    return BitConverter.ToSingle(bytes, 0);
                }
                else
                {
                    throw new UnknownProcessorTypeException("Cannot convert bytes to Float.");
                }
            }
        }

        /// <summary>
        /// Converts a single-precision floating-point value to its 32-bit unsigned integer representation in
        /// little-endian format.
        /// </summary>
        /// <param name="f">The float value to convert.</param>
        /// <returns>The 32-bit unsigned integer representation of the float in little-endian format.</returns>
        private static uint LittleEndianFloatAsUInt32(float f)
        {
            byte[] bytes = BitConverter.GetBytes(f);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Converts a little-endian IEEE 754 float to a VAX F-float byte array.
        /// </summary>
        /// <param name="a">The float value to convert.</param>
        /// <returns>A 4-byte array representing the VAX F-float equivalent.</returns>
        private static byte[] LittleEndianFloatToVaxFloatByte(float a)
        {
            byte[] b = new byte[4];
            uint t;

            // format underflow: flush to zero
            if (Math.Abs(a) < TWO_TO_M128)
            {
                t = 0;
            }
            // format overflow: clamp to maximum magnitude
            else if (Math.Abs(a) >= TWO_TO_127)
            {
                t = a < 0 ? 0xffffffff : 0x7fffffff;
            }
            // large: scale by exponent manipulation to avoid overflow in intermediates
            else if (Math.Abs(a) >= TWO_TO_126)
            {
                t = LittleEndianFloatAsUInt32(a);
                t += 2u << 23; // increment exponent by 2; equivalent multiply by 4
            }
            // common case: scale by multiplication
            else
            {
                a *= SCAL;
                t = LittleEndianFloatAsUInt32(a);
            }

            // adjust to VAX F-float byte ordering
            b[0] = (byte)(t >> 16);
            b[1] = (byte)(t >> 24);
            b[2] = (byte)(t >> 0);
            b[3] = (byte)(t >> 8);

            return b;
        }

        /// <summary>
        /// Converts a float value to its big-endian 32-bit unsigned integer representation.
        /// </summary>
        /// <param name="f">The float value to convert.</param>
        /// <returns>The big-endian 32-bit unsigned integer representation of the float.</returns>
        private static uint BigEndianFloatAsUInt32(float f)
        {
            byte[] bytes = BitConverter.GetBytes(f);
            Array.Reverse(bytes); // Convert to big-endian
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Converts a big-endian IEEE 754 float to a VAX F-float byte array representation.
        /// </summary>
        /// <param name="a">The big-endian IEEE 754 float value to convert.</param>
        /// <returns>A 4-byte array containing the VAX F-float representation.</returns>
        private static byte[] BigEndianFloatToVaxF(float a)
        {
            byte[] b = new byte[4];
            uint t;

            // format underflow: flush to zero
            if (Math.Abs(a) < TWO_TO_M128)
            {
                t = 0;
            }
            // format overflow: clamp to maximum magnitude
            else if (Math.Abs(a) >= TWO_TO_127)
            {
                t = a < 0 ? 0xffffffff : 0x7fffffff;
            }
            // large: scale by exponent manipulation to avoid overflow in intermediates
            else if (Math.Abs(a) >= TWO_TO_126)
            {
                t = BigEndianFloatAsUInt32(a);
                t += 2u << 23; // increment exponent by 2; equivalent multiply by 4
            }
            // common case: scale by multiplication
            else
            {
                a *= SCAL;
                t = BigEndianFloatAsUInt32(a);
            }

            // adjust to VAX F-float byte ordering
            b[0] = (byte)(t >> 16);
            b[1] = (byte)(t >> 24);
            b[2] = (byte)(t >> 0);
            b[3] = (byte)(t >> 8);

            return b;
        }

        /// <summary>
        /// Converts a 32-bit unsigned integer to a single-precision floating-point value using little-endian byte
        /// order.
        /// </summary>
        /// <param name="u">The 32-bit unsigned integer to convert.</param>
        /// <returns>The single-precision floating-point value represented by the specified unsigned integer.</returns>
        private static float UInt32AsLittleEndianFloat(uint u)
        {
            byte[] bytes = BitConverter.GetBytes(u);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Converts a 4-byte VAX F-float value to a little-endian IEEE 754 float.
        /// </summary>
        /// <param name="vaxBytes">A byte array containing the VAX F-float value to convert.</param>
        /// <returns>The converted little-endian float value.</returns>
        /// <exception cref="ArgumentException">Thrown when the input buffer is null or less than 4 bytes long.</exception>
        private static float VaxFToLittleEndianFloat(byte[] vaxBytes)
        {
            if (vaxBytes == null || vaxBytes.Length < 4)
                throw new ArgumentException("Buffer must be at least 4 bytes long.");

            // Reconstruct the uint32_t from VAX F-float bytes
            uint t = (uint)(
                vaxBytes[0] << 16 |
                vaxBytes[1] << 24 |
                vaxBytes[2] << 0 |
                vaxBytes[3] << 8
            );

            // Handle special cases
            if (t == 0)
                return 0.0f;
            if (t == 0xffffffff)
                return -float.MaxValue;
            if (t == 0x7fffffff)
                return float.MaxValue;

            // Reverse the scaling or exponent adjustment
            if ((t & 0x7f800000) >= 126 + 127 << 23) // If exponent was incremented by 2
            {
                t -= 2u << 23; // Decrement exponent by 2; equivalent divide by 4
                return UInt32AsLittleEndianFloat(t);
            }
            else
            {
                float f = UInt32AsLittleEndianFloat(t);
                return f / SCAL; // Reverse the multiplication by SCAL
            }
        }

        /// <summary>
        /// Converts a 32-bit unsigned integer to a single-precision floating-point value using big-endian byte order.
        /// </summary>
        /// <param name="u">The 32-bit unsigned integer to convert.</param>
        /// <returns>The resulting single-precision floating-point value.</returns>
        private static float UInt32AsBigEndianFloat(uint u)
        {
            byte[] bytes = BitConverter.GetBytes(u);
            Array.Reverse(bytes); // Convert to big-endian
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Converts a 4-byte VAX F-float value to a big-endian IEEE 754 float.
        /// </summary>
        /// <param name="vaxBytes">A byte array containing the VAX F-float value to convert.</param>
        /// <returns>The converted big-endian IEEE 754 float value.</returns>
        /// <exception cref="ArgumentException">Thrown when the input buffer is null or less than 4 bytes long.</exception>
        private static float VaxFToBigEndianFloat(byte[] vaxBytes)
        {
            if (vaxBytes == null || vaxBytes.Length < 4)
                throw new ArgumentException("Buffer must be at least 4 bytes long.");

            // Reconstruct the uint32_t from VAX F-float bytes
            uint t = (uint)(
                vaxBytes[0] << 16 |
                vaxBytes[1] << 24 |
                vaxBytes[2] << 0 |
                vaxBytes[3] << 8
            );

            // Handle special cases
            if (t == 0)
                return 0.0f;
            if (t == 0xffffffff)
                return -float.MaxValue;
            if (t == 0x7fffffff)
                return float.MaxValue;

            // Reverse the scaling or exponent adjustment
            if ((t & 0x7f800000) >= 126 + 127 << 23) // If exponent was incremented by 2
            {
                t -= 2u << 23; // Decrement exponent by 2; equivalent divide by 4
                return UInt32AsBigEndianFloat(t);
            }
            else
            {
                float f = UInt32AsBigEndianFloat(t);
                return f / SCAL; // Reverse the multiplication by SCAL
            }
        }

    }
}