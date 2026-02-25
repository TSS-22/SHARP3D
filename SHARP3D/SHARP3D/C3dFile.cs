using SHARP3D.Header;
using SHARP3D.Parameter;
using SHARP3D.Parameter.Data;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SHARP3D.Test")]
[assembly: InternalsVisibleTo("SHARP3D.Explorer")] // To remove for production
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
        public FileStream? C3dStream { get; set; } = null;
        /// <summary>
        /// Specifies the type of processor that was used to create the C3D file.
        /// </summary>
        public ProcessorType ProcessorFile { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;
        public ProcessorType ProcessorHost { get; set; } = BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS;

        public int PointerDataSection { get; set; }

        public float ScaleFactor { get; set; }

        public DataType DataType { get; set; }

        public C3dHeader Header { get; set; } = new C3dHeader();

        public List<C3dParameterGroup> Parameters { get; set; }

        public C3dParameterCollection ParameterCollection { get; set; }

        internal C3dFile() { }

        internal C3dFile(FileStream fileStream)
        {
            C3dStream = fileStream;
            ProcessorFile = ReadProcessorByte(fileStream); 
            ScaleFactor = GetScaleFactor(fileStream, ProcessorFile);
            DataType = ScaleFactor < 0 ? DataType.FLOAT32 : DataType.INT16;

            Header = GetHeader(C3dStream, ProcessorFile);

            Parameters = GetParameters(C3dStream, ProcessorFile, Header.PointerDataSection);

            ParameterCollection = new C3dParameterCollection(Parameters);

        }
        

        public C3dFile CreateEmpty()
        {
            return new C3dFile();
        }

        internal C3dHeader GetHeader(FileStream fileStream, ProcessorType processorFile)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] headerBinaries = ReadHeaderBinaries(fileStream);
            return C3dHeader.FromBinaries(headerBinaries, processorFile);
        }

        internal List<C3dParameterGroup> GetParameters(FileStream fileStream, ProcessorType processorFile, int pointerDataSection)
        {
            if (fileStream == null)
            {
                throw new InvalidOperationException("File stream is not open.");
            }
            return C3dParameterHelper.ParametersFromFileStreams(fileStream, processorFile, pointerDataSection);
        }

        public static C3dFile LoadFromFile(string filepath)
        {
            FileStream fileStream = OpenC3dFile(filepath);
            
            return new C3dFile(fileStream);
        }

        internal static int GetParameterSectionPointer(FileStream c3dStream)
        {
            byte[] pointerToParameter = new byte[1];
            c3dStream.Seek(0, SeekOrigin.Begin);
            c3dStream.ReadExactly(pointerToParameter);
            return BitConverter.ToInt16(new byte[] { 0, pointerToParameter[0] }, 0);
        }

        internal static int GetDataSectionPointer(FileStream c3dStream, ProcessorType processor)
        {
            byte[] pointerToData = new byte[2];
            c3dStream.Seek(16, SeekOrigin.Begin);
            c3dStream.ReadExactly(pointerToData);
            return C3dBytesConvertor.ToInt(pointerToData, processor);
        }

        // Not usefull but here in case of
        internal static int GetParameterBlockCount(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 2, SeekOrigin.Begin);
            return c3dStream.ReadByte();
        }
        internal static ProcessorType ReadProcessorByte(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 3, SeekOrigin.Begin);
            return (ProcessorType)c3dStream.ReadByte();
        }

        internal float GetScaleFactor(FileStream c3dStream, ProcessorType processor)
        {
            byte[] valueBuffer = new byte[4];
            c3dStream.Seek(12, SeekOrigin.Begin);
            c3dStream.ReadExactly(valueBuffer);
            return C3dBytesConvertor.ToFloat(valueBuffer, processor);
        }

        internal static byte[] ReadHeaderBinaries(FileStream c3dStream)
        {
            byte[] headers = new byte[512];
            c3dStream.ReadExactly(headers, 0, 512);
            return headers;
        }

        internal static byte[] ReadParameterBinaries(FileStream c3dStream, int parameterSectionPointer, int parameterBlockCount)
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

        internal static FileStream OpenC3dFile(string filepath)
        {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read);
        }

        public bool IsFileStreamOpen()
        {
            return C3dStream != null;
        }

        public void CloseFileStream()
        {
            if (C3dStream != null)
            {
                C3dStream.Close();
                C3dStream = null;
            }
        }
    }
}
