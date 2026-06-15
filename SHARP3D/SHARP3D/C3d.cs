using SHARP3D.Data.DataEntity;
using SHARP3D.Parameter.DataEntity;
using SHARP3D.Parameter.DataEntity.Clean;

namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;
        public C3dParameterPoint RequiredPoint = new C3dParameterPoint();
        public C3dParameterAnalog RequiredAnalog = new C3dParameterAnalog();

        public C3dParameterSection Parameters;

        public C3dData Data = new C3dData(); 

        public C3d(string filePath)
        {
            C3dFile c3DFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3DFile.FilePath;
            RequiredPoint = c3DFile.Point;
            RequiredAnalog = c3DFile.Analog;

            (Data.Point, Data.Residual, Data.CameraMask) = GetPointDataFromFile();
            (Data.Analog, Data.ForcePlate) = GetAnalogDataFromFile();

        }

        public C3d() { }

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
