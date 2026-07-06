using SHARP3D.Data.Clean;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils;

namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;
        public C3dRequiredParameters Required = new C3dRequiredParameters();

        public C3dParameterSection Parameters = new C3dParameterSection();

        public C3dData Data = new C3dData();

        public C3d() { }

        public C3d(C3dData data, C3dParameterSection? parameters = null, C3dRequiredParameters? required = null)
        {
            Data = data;
            Required = required != null ? required : new C3dRequiredParameters(); 
            Parameters = parameters != null ? parameters : new C3dParameterSection();
        }

        public C3d(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3dFile.FilePath;
            
            Required.Point = new C3dParameterPoint { 
                Rate = c3dFile.Point.Rate,
                Units = c3dFile.Point.Units,
                Frames = c3dFile.Point.Frames,
            };
            Required.Analog = new C3dParameterAnalog{
                GeneralScale = c3dFile.Analog.GeneralScale,
                SamplesPerFrame = c3dFile.Analog.SamplesPerFrame,
            };

            Parameters = new C3dParameterSection(c3dFile.Parameters);

            Data = new C3dData(c3dFile);

            Parameters.DeleteUneededParametersFromFiles();
        }

        public void ChangePointUnit(string newUnit, float factor)
        {
            Required.Point.Units = newUnit;
            foreach(C3dPointTrajectory trajectory in Data.Points)
            {
                for (int i = 0; i < trajectory.Point.GetLength(0); i++)
                {
                    for(int j=0; j< trajectory.Point.GetLength(1); j++)
                    {
                        trajectory.Point[i,j] = trajectory.Point[i,j] * factor;
                    }
                    trajectory.Residual[i] = trajectory.Residual[i] * factor;
                }
            }
            
        }

        public void ChangeAnalogGeneralScale(float generalScale)
        {
            foreach(C3dAnalogChannel channel in Data.Analogs)
            {
                for(int i=0; i < channel.Data.Length; i++)
                {
                    channel.Data[i] = channel.Data[i] / Required.Analog.GeneralScale * generalScale;
                }
            }

            for(int idPlate = 0; idPlate<Data.Forceplates.Length;idPlate++)
            {
                foreach (C3dAnalogChannel channel in Data.Forceplates[idPlate].Channels)
                {
                    for (int i = 0; i < channel.Data.Length; i++)
                    {
                        channel.Data[i] = channel.Data[i] / Required.Analog.GeneralScale * generalScale;
                    }
                }
            }

            Required.Analog.GeneralScale = generalScale;    
        }

        

        // TODO
        public int AddFrame()
        {
            return Data.Points.Length != 0 ? Data.Points[0].Residual.Length : 0;
        }

        // TODO
        public int DeleteFrame(int idFrameToDelete)
        {
            foreach (C3dPointTrajectory trajectory in Data.Points)
            {

            }

            foreach (C3dAnalogChannel channel in Data.Analogs)
            {

            }

            foreach (C3dForceplate forcePlate in Data.Forceplates)
            {

            }

            return Data.Points.Length != 0 ? Data.Points[0].Residual.Length : 0;
        }
    }
}
