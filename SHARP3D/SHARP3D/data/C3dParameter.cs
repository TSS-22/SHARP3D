namespace SHARP3D
{
    ///<summary>
    ///This structure regroup the C3D parameters from the file. They determine the endian format used. For some very logical reasons they need to be processed before the header could be processed.
    ///</summary>
    public struct C3dParameter {

        ///<summary>
        /// <para>Byte: 3</para>
        /// The number of parameter blocks (512 bytes) in the parameter section.
        /// </summary>
        public int ParameterBlockCount;
        ///<summary>
        ///<para>Byte: 4</para>
        ///The type of processor used to write the C3D file. It determine the way data is stored in the file. At the moment only Intel, DEC and SGI/MIPS CPU are supported. It is supposed they could be added on request to the C3D organisation.
        ///</summary>
        public int FileMakerProcessorType;

        // TODO: Implement method to parse binaries into C3dParameter struct.
        public static C3dParameter FromBinaries(byte[] binaries)
        {
            return new C3dParameter();
        }

        // TODO: Implement method to convert C3dParameter struct into binaries.
        public static byte[] ToBinaries()
        {
            return new byte[0];
        }
    }
}