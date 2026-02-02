using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using SHARP3D;

namespace SHARP3D.Explorer
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var path = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d";
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File not found: {path}");
                return 1;
            }
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                byte[] header = C3dFileManager.ReadHeader(fs);
                foreach (byte b in header)
                {
                    Console.Write($"{b:X2} ");
                }

                return 0;
            }
            catch (Exception ex) 
            { 
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }

        }
    }
}