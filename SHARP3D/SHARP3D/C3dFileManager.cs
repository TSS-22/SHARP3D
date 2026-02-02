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
    }
}
