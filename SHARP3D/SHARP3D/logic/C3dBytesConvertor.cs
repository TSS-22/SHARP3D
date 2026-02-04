namespace SHARP3D
{
    /// <summary>
    /// This helper class provide functions to process C3D bytes according to the file maker processor type for compatibility purposes.
    /// </summary>
    /// <para>
    /// The Intel floating point format is little-endian IEEE 754.
    /// </para>
    public static class C3dBytesConvertor
    {
        public static float ConvertBytesToFloat(byte[] bytes, ProcessorType processorMakerType)
        {
            if (processorMakerType != ProcessorType.INTEL &&
               processorMakerType != ProcessorType.DEC &&
               processorMakerType != ProcessorType.SIG_MIPS)
            {
                // TODO: Handle unknown processor type
            }
            if (BitConverter.IsLittleEndian)
            {
                if (processorMakerType == ProcessorType.INTEL)
                {

                }
                else if (processorMakerType == ProcessorType.DEC)
                {

                }
                else if (processorMakerType == ProcessorType.SIG_MIPS)
                {

                }
            }
            else
            {
                if (processorMakerType == ProcessorType.INTEL)
                {

                }
                else if (processorMakerType == ProcessorType.DEC)
                {

                }
                else if (processorMakerType == ProcessorType.SIG_MIPS)
                {

                }
            }
            return 0.0f;
        }
    }
}