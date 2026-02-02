namespace SHAPR3D
{
    
    public class C3dFile
    {
        byte[] block_header;
        byte[] block_parameter;
        public C3dFile(byte[] header, byte[] parameter)
        {
            this.block_header = header;
            this.block_parameter = parameter;
        }

        static public ProcessHeader(byte[] header)
        {
            //Process the header block here
        }

    }
}
