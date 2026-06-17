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

        public C3dParameterSection Parameters = new C3dParameterSection();

        public C3dData Data = new C3dData(); 

        public C3d(string filePath)
        {
            C3dFile c3DFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3DFile.FilePath;
            
            RequiredPoint = c3DFile.Point;
            RequiredAnalog = c3DFile.Analog;
            Parameters = new C3dParameterSection(c3DFile.Parameters);

            Data = GetDataFromFile(c3DFile.Data);

        }

        public C3d() { }

        internal C3dData GetDataFromFile(C3dFileData fileData) 
        {
            C3dData data = new C3dData();
            if(fileData.Points.Count != 0)
            {
                (data.Point, data.Residual, data.CameraMask) = GetPointDataFromFile(fileData.Points);
            }
            if(fileData.Analogs.Count != 0)
            {
                (data.Analog, data.ForcePlate) = GetAnalogDataFromFile(fileData.Analogs);
            }
            return data;
        }
        internal (float?[,,], float?[,], bool[,,]) GetPointDataFromFile(List<C3dFileDataPoint[]> filePointData)
        {
            int nbFrame = filePointData.Count;
            int nbTrajectory = filePointData[0].Length;
            int nbPoint = filePointData[0][0].Point.Length;
            int nbCameraMask = filePointData[0][0].CameraMask.Length;

            float?[,,] point = new float?[nbFrame, nbTrajectory, nbPoint];
            float?[,] residual = new float?[nbFrame, nbTrajectory ];
            bool[,,] cameraMask = new bool[nbFrame, nbTrajectory, nbCameraMask];

            for(int idFrame=0; idFrame < nbFrame; idFrame++)
            {
                for(int idTraj=0; idTraj < nbTrajectory; idTraj++)
                {
                    // Point populating
                    if (filePointData[idFrame][idTraj].Valid != null)
                    {
                        for (int idPoint=0; idPoint < nbPoint; idPoint++) 
                        {
                            point[idFrame, idTraj, idPoint] = filePointData[idFrame][idTraj].Point[idPoint];
                        }
                    }

                    // Residual populating
                    if (filePointData[idFrame][idTraj].Raw!=null)
                    {
                        residual[idFrame, idTraj] = filePointData[idFrame][idTraj].AverageResidual;
                    }

                    // Camera Mask populating
                    for(int idMask=0; idMask < nbCameraMask; idMask++)
                    {
                        cameraMask[idFrame, idTraj, idMask] = filePointData[idFrame][idTraj].CameraMask[idMask];
                    }
                    
                }
            }
            return (point, residual, cameraMask);
        }

        // We are making the bet that going by frame is the right choice. Should be easier to put back in binaries maybe ?
        // I will make a function to get the analog in a simple 2D array
        internal (float[,], float[,,]?) GetAnalogDataFromFile(List<float[][]> fileAnalogData)
        {
            int nbFrame = fileAnalogData.Count * RequiredAnalog.SamplesPerFrame;

            float[,] analog = new float[nbFrame, RequiredAnalog.Used];

            for (int idFrame = 0; idFrame < nbFrame; idFrame++)
            {
                for (int idSample = 0; idSample < RequiredAnalog.SamplesPerFrame; idSample++)
                {
                    for (int idChannel = 0; idChannel < RequiredAnalog.Used; idChannel++)
                    {
                        analog[idFrame * 4 + idSample, idChannel] = fileAnalogData[idFrame][idSample][idChannel];
                    }
                }
            }

                    return (analog, null);
        }

        internal float[,,] GetForcePlateDataFromFile()
        {
            return new float[,,] { };
        }
    }
}
