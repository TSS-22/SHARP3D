using SHARP3D.Data.Clean;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils

using System.Linq;

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

        public C3d Concat(C3d object1,  C3d object2, bool rowConcat = true)
        {
            // object1 got power of object2 in case of resolvable conflict

            // PARAMETER CHECK

            // POINTS checks
            // Check if same units
            // Check the rate
            if (object1.Required.Point.Rate != object2.Required.Point.Rate)
            {
                throw new ArgumentException($"POINT:RATE values are not compatible: {object1.Required.Point.Rate} | {object2.Required.Point.Rate} ");
            }
            if (object1.Required.Point.Units.ToUpper() != object2.Required.Point.Units.ToUpper())
            {
                throw new ArgumentException($"POINT:UNITS values are not compatible: {object1.Required.Point.Units} | {object2.Required.Point.Units} ");
            }
            C3dParameterPoint newRequiredPoint = new C3dParameterPoint 
            {
                Rate = object1.Required.Point.Rate,
                Units = object2.Required.Point.Units,
                Frames = object1.Required.Point.Frames + object2.Required.Point.Frames,
            };

            // ROW CONCAT
            if (rowConcat) 
            {
                C3dRequiredParameters parameters = new C3dRequiredParameters();
                // Associate per Labels, use the description of object1
                List<int> singleTrajectoriesC3d1 = new List<int>();
                List<int> singleTrajectoriesC3d2 = new List<int>();
                List<(int, int)> partnerTrajectories = new List<(int, int)>();

                for (int label1 = 0; label1 < object1.Data.Points.Length; label1++)
                {
                    bool partnerFound = false;
                    for (int label2 = 0; label2 < object2.Data.Points.Length; label2++)
                    {
                        if (object2.Data.Points[label2].Label == object1.Data.Points[label1].Label)
                        {
                            partnerTrajectories.Add((label1, label2));
                            partnerFound = true;
                            break;
                        }
                    }
                    if (!partnerFound)
                    {
                        singleTrajectoriesC3d1.Add(label1);
                    }
                }
                for (int label2 = 0; label2 < object2.Data.Points.Length; label2++)
                {
                    bool partnerFound = false;
                    foreach ((int, int) partners in partnerTrajectories)
                    {
                        if (partners.Item2 == label2)
                        {
                            partnerFound = true;
                            break;
                        }
                    }
                    if (!partnerFound)
                    {
                        singleTrajectoriesC3d2.Add(label2);
                    }
                }
                // Concat data of each partner and if somebody is missing labels, Add invalid points to it
                List<C3dPointTrajectory> newPointTrajectories = new List<C3dPointTrajectory>();
                // Add partnered trajectories
                foreach ((int, int) idPartners in partnerTrajectories)
                {
                    List<float?[]> newPointData = new List<float?[]>();
                    List<bool[]> newCameraMaskData = new List<bool[]>();

                    for(int idSample = 0; idSample < object1.Data.Points[idPartners.Item1].Residual.Length; idSample++)
                    {
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for(int idAxis = 0; idAxis < object1.Data.Points[idPartners.Item1].Point.GetLength(1); idAxis++)
                        {
                            tempListPoint.Add(object1.Data.Points[idPartners.Item1].Point[idSample, idAxis]);
                        }
                        newPointData.Add(tempListPoint.ToArray());

                        // Mask
                        List<bool> tempListMask = new List<bool>();
                        for (int idCamera = 0; idCamera < object1.Data.Points[idPartners.Item1].Point.GetLength(1); idCamera++)
                        {
                            tempListMask.Add(object1.Data.Points[idPartners.Item1].CameraMask[idSample, idCamera]);
                        }
                        newCameraMaskData.Add(tempListMask.ToArray());
                    }

                    for (int idSample = 0; idSample < object2.Data.Points[idPartners.Item2].Residual.Length; idSample++)
                    {
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for (int idAxis = 0; idAxis < object2.Data.Points[idPartners.Item2].Point.GetLength(1); idAxis++)
                        {
                            tempListPoint.Add(object2.Data.Points[idPartners.Item2].Point[idSample, idAxis]);
                        }
                        newPointData.Add(tempListPoint.ToArray());

                        // Mask
                        List<bool> tempListMask = new List<bool>();
                        for (int idCamera = 0; idCamera < object2.Data.Points[idPartners.Item2].CameraMask.GetLength(1); idCamera++)
                        {
                            tempListMask.Add(object2.Data.Points[idPartners.Item2].CameraMask[idSample, idCamera]);
                        }
                        newCameraMaskData.Add(tempListMask.ToArray());
                    }

                    newPointTrajectories.Add( new C3dPointTrajectory 
                    {
                        Label = object1.Data.Points[idPartners.Item1].Label,
                        Description= object1.Data.Points[idPartners.Item1].Description,
                        Point = newPointData.To2DArray(),
                        Residual = object1.Data.Points[idPartners.Item1].Residual.Concat(object2.Data.Points[idPartners.Item2].Residual).ToArray(),
                        CameraMask = newCameraMaskData.To2DArray()
                    });
                }

                // Add single trajectories from object1
                foreach (int idSingle in singleTrajectoriesC3d1)
                {
                    List<float?[]> newPointData = new List<float?[]>();
                    List<float?> newResidualData = new List<float?>();
                    List<bool[]> newCameraMaskData = new List<bool[]>();

                    for (int idSample = 0; idSample < object1.Data.Points[idSingle].Residual.Length; idSample++)
                    {
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for (int idAxis = 0; idAxis < object1.Data.Points[idSingle].Point.GetLength(1); idAxis++)
                        {
                            tempListPoint.Add(object1.Data.Points[idSingle].Point[idSample, idAxis]);
                        }
                        newPointData.Add(tempListPoint.ToArray());

                        newResidualData.Add(null);

                        // Mask
                        List<bool> tempListMask = new List<bool>();
                        for (int idCamera = 0; idCamera < object1.Data.Points[idSingle].CameraMask.GetLength(1); idCamera++)
                        {
                            tempListMask.Add(object1.Data.Points[idSingle].CameraMask[idSample, idCamera]);
                        }
                        newCameraMaskData.Add(tempListMask.ToArray());
                    }

                    for (int idSample = 0; idSample < object2.Required.Point.Frames; idSample++)
                    {
                        // Need to check for number of axis and number of camera first
                    }

                    newPointTrajectories.Add(new C3dPointTrajectory
                    {
                        Label = object1.Data.Points[idSingle].Label,
                        Description = object1.Data.Points[idSingle].Description,
                        Point = newPointData.To2DArray(),
                        Residual = newResidualData.ToArray(),
                        CameraMask = newCameraMaskData.To2DArray()
                    });

                }
                // Add single trajectories from object2

                // ANALOG
                // Associate per Labels, use the description of object1
                // Check if same units
                // Check if same rate

                // If somebody is missing labels association, add Offset valued sample

                // FORCEPLATE
                // Do analog check on the data

                // Check same type. Corner. Origin etc
                }
            // COL CONCAT
            else
            {
                
                // Check if they have the same number of sample in every trajectories

                // ANALOG & FORCEPLATE
                // Check general scale. Can probably change the general scale thanks to Descale/Rescale. and not be an error
                // Check if same length of data
            }

            return new C3d();
        }
    }
}
