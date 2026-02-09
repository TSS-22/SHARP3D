namespace SHARP3D
{
    /// <summary>
    /// Represents a C3D file, providing methods for processing headers, parameters, loading, saving, and binary
    /// conversion.
    /// </summary>
    public class C3dFile
    {
        /// <summary>
        /// Represents the file stream used to access the C3D file.
        /// </summary>
        FileStream? fileStream { get; set; } = null;
        /// <summary>
        /// Specifies the type of processor that was used to create the C3D file.
        /// </summary>
        ProcessorType processorFileType { get; set; } = BitConverter.IsLittleEndian? ProcessorType.INTEL : ProcessorType.SIG_MIPS;
        ProcessorType processorType { get; set; } = BitConverter.IsLittleEndian? ProcessorType.INTEL : ProcessorType.SIG_MIPS;
        
        C3dHeader c3DHeader { get; set; } = new C3dHeader();

        private C3dFile(){ }

        private C3dFile(FileStream fileStream, ProcessorType processorMakerType)
        {
            this.fileStream = fileStream;
            this.processorFileType = processorMakerType;
            
            this.fileStream.Seek(0, SeekOrigin.Begin);
            byte[] headerBinaries = ReadHeader(this.fileStream);
            this.c3DHeader = C3dHeader.FromBinaries(headerBinaries);
        }

        public C3dFile CreateEmpty()
        {
            return new C3dFile();
        }

        internal C3dParameter ProcessParameterBytes(byte[] parameterBytes)
        {
            return C3dParameter.FromBinaries(parameterBytes);
        }

        public static C3dFile LoadFromFile(string filepath)
        {
            FileStream fileStream = OpenC3dFile(filepath);
            ProcessorType processorMakerType = ReadProcessorByte(fileStream);
            return new C3dFile(fileStream, processorMakerType);
        }

        public static int GetParameterSectionPointer(FileStream c3dStream)
        {
            byte[] pointerToParameter = new byte[1];
            c3dStream.Seek(0, SeekOrigin.Begin);
            c3dStream.ReadExactly(pointerToParameter, 0, 1);
            return BitConverter.ToInt16(new byte[] { 0, pointerToParameter[0] }, 0);
        }
        public static ProcessorType ReadProcessorByte(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 3, SeekOrigin.Begin);
            return (ProcessorType)c3dStream.ReadByte();
        }

        public static byte[] ReadHeader(FileStream c3dStream)
        {
            byte[] headers = new byte[512];
            c3dStream.ReadExactly(headers, 0, 512);
            return headers;
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

        public static FileStream OpenC3dFile(string filepath)
        {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read);
        }
    }
}
