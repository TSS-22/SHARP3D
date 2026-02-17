using SHARP3D.Header;
using SHARP3D.Parameter;
using SHARP3D.Utils.Enum;

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
        FileStream? FileStream { get; set; } = null;
        /// <summary>
        /// Specifies the type of processor that was used to create the C3D file.
        /// </summary>
        public ProcessorType ProcessorFileType { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;
        public ProcessorType ProcessorHostType { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;

        public C3dHeader C3DHeader { get; set; } = new C3dHeader();

        public C3dParameterBlock Parameters { get; set; }

        private C3dFile() { }

        private C3dFile(FileStream fileStream, ProcessorType processorMakerType)
        {
            this.FileStream = fileStream;
            this.ProcessorFileType = processorMakerType;

            this.FileStream.Seek(0, SeekOrigin.Begin);
            byte[] headerBinaries = ReadHeaderBinaries(this.FileStream);
            this.C3DHeader = C3dHeader.FromBinaries(headerBinaries, processorMakerType);

            byte[] parameterBinaries = ReadParameterBinaries(this.FileStream, GetParameterSectionPointer(this.FileStream), GetParameterBlockCount(this.FileStream));
            this.Parameters = ProcessParameterBytes();
        }

        public C3dFile CreateEmpty()
        {
            return new C3dFile();
        }

        internal C3dParameterBlock ProcessParameterBytes()
        {
            if (FileStream == null)
            {
                throw new InvalidOperationException("File stream is not open.");
            }
            return C3dParameterBlock.FromFileStream(FileStream, ProcessorFileType);
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

        public static int GetParameterBlockCount(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 2, SeekOrigin.Begin);
            return c3dStream.ReadByte();
        }
        public static ProcessorType ReadProcessorByte(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 3, SeekOrigin.Begin);
            return (ProcessorType)c3dStream.ReadByte();
        }

        public static byte[] ReadHeaderBinaries(FileStream c3dStream)
        {
            byte[] headers = new byte[512];
            c3dStream.ReadExactly(headers, 0, 512);
            return headers;
        }

        public static byte[] ReadParameterBinaries(FileStream c3dStream, int parameterSectionPointer, int parameterBlockCount)
        {
            byte[] parameters = new byte[parameterBlockCount * 512];
            c3dStream.Seek(parameterSectionPointer, SeekOrigin.Begin);
            c3dStream.ReadExactly(parameters, 0, parameterBlockCount * 512);
            return parameters;
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

        public bool IsFileStreamOpen()
        {
            return FileStream != null;
        }

        public void CloseFileStream()
        {
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream = null;
            }
        }
    }
}
