namespace SHARP3D
{
    /// <summary>
    /// Represents a C3D file, providing methods for processing headers, parameters, loading, saving, and binary
    /// conversion.
    /// </summary>
    public class C3dFile
    {
        ProcessorType processorType;
        // TODO: Implement constructor logic.
        public C3dFile(byte[] c3dBinairies)
        {
            //C3dFileManager.ReadProcessorByte(c3dBinairies);

        }

        // TODO: Implement actual header processing logic.
        public static C3dHeader ProcessHeaderBytes(byte[] headerBytes)
        {
            return C3dHeader.FromBinaries(headerBytes);
        }

        // TODO: Implement actual parameter processing logic.
        public static C3dParameter ProcessParameterBytes(byte[] parameterBytes)
        {
            return C3dParameter.FromBinaries(parameterBytes);
        }

        // TODO: Implement actual file loading logic. Use the C3dFileManager for this
        public static C3dFile LoadFromFile(string filepath)
        {
            return new C3dFile(new byte[] { });
        }

        // TODO: Implement actual binaries transformation logic.
        public byte[] ToBinaries()
        {
            return new byte[] { };
        }

        // TODO: Implement actual file saving logic. Return 0 if success, else error code. Use the C3dFileManager for this
        public int SaveToFile(string filepath)
        {
            return 0;
        }


    }
}
