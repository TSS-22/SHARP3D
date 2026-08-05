namespace SHARP3D.Utils
{
    public static class BitManipulator
    {
        public static byte SetBit(byte b, int position, bool value)
        {
            if (value)
                return (byte)(b | (1 << position));
            else
                return (byte)(b & ~(1 << position));
        }

        public static bool GetBit(byte b, int position)
        {
            return (b & (1 << position)) != 0;
        }

        public static byte ToggleBit(byte b, int position)
        {
            return (byte)(b ^ (1 << position));
        }

        public static byte GetByteFromBits(bool[] bits)
        {
            if (bits.Length > 8) throw new ArgumentException("Only 8 bits supported");
            byte result = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                    result |= (byte)(1 << i);
            }
            return result;
        }
    }
}
