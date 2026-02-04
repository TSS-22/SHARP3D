namespace SHARP3D
{
    
    public class C3dFile
    {
        // TODO: Implement constructor logic.
        public C3dFile(byte[] c3dBinairies)
        {
            
            
        }

        /// <summary>
        /// Read the first byte of the file to determine the pointer to the parameter section where the processor type word is stored.
        /// </summary>
        /// <param name="c3dBinairies">The C3D file binaries</param>
        /// <returns>Return the processor type used to create the C3D file.</returns>
        public static ProcessorType discoverProcessorType(byte[] c3dBinairies)
        {
            if (c3dBinairies.Length == 0)
            {
                throw new ArgumentException("The C3D binaries cannot be empty.");
            }
            // The first byte of the file is the pointer to the parameter section.
            int parameterSectionPointer = BitConverter.ToInt16(new byte[] { c3dBinairies[0], 0 },0);
            // The processor type word is located at the 4th byte of the parameter section.
            int processorTypeWordIndex = parameterSectionPointer + 4;
            if (processorTypeWordIndex >= c3dBinairies.Length)
            {
                throw new ArgumentException("The C3D binaries are not valid. The processor type word index is out of bounds.");
            }
            int processorTypeWord = c3dBinairies[processorTypeWordIndex];
            return (ProcessorType)processorTypeWord;
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
