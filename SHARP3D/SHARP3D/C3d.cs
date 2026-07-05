using SHARP3D.Data.Clean;
using SHARP3D.Parameter.DataEntity.Clean;


namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;
        public C3dRequiredParameters Required = new C3dRequiredParameters();

        public C3dParameterSection Parameters = new C3dParameterSection();

        public C3dData Data = new C3dData();

        public C3d() { }

        public C3d(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3dFile.FilePath;
            
            Required.Point = new C3dParameterPoint { 
                Rate = c3dFile.Point.Rate,
                Units = c3dFile.Point.Units,
            };
            Required.Analog = new C3dParameterAnalog{
                GeneralScale = c3dFile.Analog.GeneralScale,
                SamplesPerFrame = c3dFile.Analog.SamplesPerFrame,
            };

            Parameters = new C3dParameterSection(c3dFile.Parameters);

            Data = new C3dData(c3dFile);

            Parameters.DeleteUneededParametersFromFiles();
        }

        public void ChangeUnit(string newUnit, float factor)
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
    }
}
