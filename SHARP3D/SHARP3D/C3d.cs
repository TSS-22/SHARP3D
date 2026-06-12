using SHARP3D.Parameter.DataEntity;
using System.Runtime.CompilerServices;

namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;
        public C3dParameterPoint Point;
        public C3dParameterAnalog Analog;
        
        public C3d(string filePath)
        {
            C3dFile c3DFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3DFile.FilePath;
            Point = c3DFile.Point;
            Analog = c3DFile.Analog;


        }

        internal (float[,,], float[]) GetPointDataFromFile()
        {
            return (new float[,,] { }, new float[] { });
        }

        // I think this one is useless.
        internal (float[], float[]?) GetAnalogDataFromFile()
        {
            return (new float[] { }, new float[] { });
        }

        internal float[] GetForcePlateDataFromFile()
        {
            return new float[] { };
        }
    }
}
