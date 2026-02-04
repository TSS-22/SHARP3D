using System.Diagnostics;

namespace SHARP3D
{
    public static class C3dFileManager
    {

        public static byte[] ReadHeader(FileStream c3dStream)
        {
            byte[] headers = new byte[512];
            c3dStream.ReadExactly(headers, 0, 512);
            return headers;
        }

        public static int GetParameterSectionPointer(FileStream c3dStream)
        {
            byte[] pointerToParameter = new byte[1];
            c3dStream.Seek(0, SeekOrigin.Begin);
            c3dStream.ReadExactly(pointerToParameter, 0, 1);
            return BitConverter.ToInt16(new byte[] { 0, pointerToParameter[0]}, 0);
        }


        public static ProcessorType ReadProcessorByte(FileStream c3dStream)
        {
            int parameterSectionPointer = GetParameterSectionPointer(c3dStream);
            c3dStream.Seek(parameterSectionPointer + 3, SeekOrigin.Begin);
            return (ProcessorType)c3dStream.ReadByte();
        }

    }
}
