using SHARP3D.Data.DataEntity;
using SHARP3D.Parameter.DataEntity;

namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;
        public C3dParameterPoint RequiredPoint;
        public C3dParameterAnalog RequiredAnalog;
        public C3dData Data; 

        public C3d(string filePath)
        {
            C3dFile c3DFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3DFile.FilePath;
            RequiredPoint = c3DFile.Point;
            RequiredAnalog = c3DFile.Analog;

            (Data.Point, Data.Residual, Data.CameraMask) = GetPointDataFromFile();
            (Data.Analog, Data.ForcePlate) = GetAnalogDataFromFile();

        }

        internal (float?[,,]?, float?[]?, bool[,,,,,,]?) GetPointDataFromFile()
        {
            return (null, null, null);
        }

        // I think this one is useless.
        internal (float[]?, float[,,]?) GetAnalogDataFromFile()
        {
            return (null, null);
        }

        internal float[,,] GetForcePlateDataFromFile()
        {
            return new float[,,] { };
        }
    }
}
