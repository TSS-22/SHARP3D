using System;
using System.Buffers.Binary;
using System.IO;

namespace SHARP3D.Explorer
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var path = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d";
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File not found: {path}");
                return 1;
            }
            try
            {
                var fs = new FileStream(path, FileMode.Open);
                var len = (int)fs.Length;
                var bits = new byte[len];
                fs.Read(bits, 0, len);
                for (int ix = 0; ix < 2; ix += 1)
                {
                    
                    Console.Write($"{BinaryConverters.ReadAscii(bits, ix, 4)} \n");
                }
                    return 0;
            }
            catch (Exception ex) 
            { 
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }

        }
    }

    public static class BinaryConverters
    {
        public static ushort ReadUInt16(ReadOnlySpan<byte> src, bool littleEndian = true)
        {
            if (src.Length < 2) throw new ArgumentException("Need at least 2 bytes", nameof(src));
            return littleEndian
                ? BinaryPrimitives.ReadUInt16LittleEndian(src)
                : BinaryPrimitives.ReadUInt16BigEndian(src);
        }

        public static ushort ReadUInt16(byte[] buffer, int offset, bool littleEndian = true) =>
            ReadUInt16(new ReadOnlySpan<byte>(buffer, offset, 2), littleEndian);

        public static short ReadInt16(ReadOnlySpan<byte> src, bool littleEndian = true) =>
            (short)ReadUInt16(src, littleEndian);

        public static short ReadInt16(byte[] buffer, int offset, bool littleEndian = true) =>
            ReadInt16(new ReadOnlySpan<byte>(buffer, offset, 2), littleEndian);

        public static string ToHex(ReadOnlySpan<byte> src)
        {
            var len = Math.Min(src.Length, 2);
            return "0x" + Convert.ToHexString(src.Slice(0, len));
        }

        public static float ReadSingle(ReadOnlySpan<byte> src, bool littleEndian = true)
        {
            if (src.Length < 4) throw new ArgumentException("Need at least 4 bytes", nameof(src));
            uint bits = littleEndian
                ? BinaryPrimitives.ReadUInt32LittleEndian(src)
                : BinaryPrimitives.ReadUInt32BigEndian(src);
            return BitConverter.Int32BitsToSingle(unchecked((int)bits));
        }

        public static float ReadSingle(byte[] buffer, int offset, bool littleEndian = true) =>
            ReadSingle(new ReadOnlySpan<byte>(buffer, offset, 4), littleEndian);

        public static string ReadAscii(ReadOnlySpan<byte> src, int count = 2)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (src.Length < count) throw new ArgumentException($"Need at least {count} bytes", nameof(src));

            if (count == 0) return string.Empty;

            Span<char> chars = count <= 256 ? stackalloc char[count] : new char[count];
            for (int i = 0; i < count; i++)
            {
                byte b = src[i];
                // printable ASCII range 0x20..0x7E; replace others with '.'
                chars[i] = (b >= 0x20 && b <= 0x7E) ? (char)b : '.';
            }

            return new string(chars);
        }

        public static string ReadAscii(byte[] buffer, int offset, int count = 2) =>
            ReadAscii(new ReadOnlySpan<byte>(buffer, offset, count), count);
    }
}