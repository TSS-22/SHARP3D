namespace SHARP3D
{
    public static class C3dFileManager
    {


        public static void OpenFile(string path)
        {
            //Will manage C3D files opening here
        }

        public static byte[] ReadHeader(FileStream c3dStream)
        {
            byte[] headers = new byte[512];
            c3dStream.ReadExactly(headers, 0, 512);
            return headers;
        }

        public static int GetParameterSectionPointer(FileStream c3dStream)
        {
            byte[] pointerToParameter = new byte[1];
            c3dStream.ReadExactly(pointerToParameter, 0, 1);
            return BitConverter.ToInt16(new byte[] { pointerToParameter[0], 0 }, 0);
        }

        public static byte ReadProcessorByte(FileStream c3dStream)
        {
            byte[] processorByte = new byte[1];
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.ReadExactly(
                processorByte,
                parameterSectionPointer + 3,
                4);
            return processorByte[0];
        }

    }
}
