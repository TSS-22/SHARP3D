using SHARP3D.Data.Clean;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils;

namespace SHARP3D
{
    internal class C3dUtils
    {
        public static C3d Concat(C3d object1, C3d object2, bool rowConcat = true)
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
            // Check the point axis dimensions
            int object1PointAxisNumber = 0;
            bool firstChek = true;
            foreach (C3dPointTrajectory trajectory in object1.Data.Points)
            {
                if (!firstChek)
                {
                    if (object1PointAxisNumber == trajectory.Point.GetLength(1))
                    {
                        continue;
                    }
                    else
                    {
                        throw new ArgumentException("Object 1 trajectories don't all have the same number of axis.");
                    }
                }
                else
                {
                    object1PointAxisNumber = trajectory.Point.GetLength(1);
                }

            }
            int object2PointAxisNumber = 0;
            firstChek = true;
            foreach (C3dPointTrajectory trajectory in object2.Data.Points)
            {
                if (!firstChek)
                {
                    if (object2PointAxisNumber == trajectory.Point.GetLength(1))
                    {
                        continue;
                    }
                    else
                    {
                        throw new ArgumentException("Object 2 trajectories don't all have the same number of axis.");
                    }
                }
                else
                {
                    object2PointAxisNumber = trajectory.Point.GetLength(1);
                }

            }
            if (object1PointAxisNumber != object2PointAxisNumber)
            {
                throw new ArgumentException("Object 1 and object 2 trajectories have different number of axis.");
            }
            // Check the camera number
            int object1CameraNumber = 0;
            firstChek = true;
            foreach (C3dPointTrajectory trajectory in object1.Data.Points)
            {
                if (!firstChek)
                {
                    if (object1PointAxisNumber == trajectory.CameraMask.GetLength(1))
                    {
                        continue;
                    }
                    else
                    {
                        throw new ArgumentException("Object 1 trajectories don't all have the same number of camera recording.");
                    }
                }
                else
                {
                    object1PointAxisNumber = trajectory.CameraMask.GetLength(1);
                }

            }
            int object2CameraNumber = 0;
            firstChek = true;
            foreach (C3dPointTrajectory trajectory in object2.Data.Points)
            {
                if (!firstChek)
                {
                    if (object2CameraNumber == trajectory.CameraMask.GetLength(1))
                    {
                        continue;
                    }
                    else
                    {
                        throw new ArgumentException("Object 2 trajectories don't all have the same number of camera recording.");
                    }
                }
                else
                {
                    object2CameraNumber = trajectory.CameraMask.GetLength(1);
                }

            }
            if (object1CameraNumber != object2CameraNumber)
            {
                throw new ArgumentException("Object 1 and object 2 have different number camera recording.");
            }
            // Define Required.Point after the checks
            C3dParameterPoint newRequiredPoint = new C3dParameterPoint
            {
                Rate = object1.Required.Point.Rate,
                Units = object2.Required.Point.Units,
                Frames = object1.Required.Point.Frames + object2.Required.Point.Frames,
            };

            // ANALOG checks
            // General Scale
            if (object1.Required.Analog.GeneralScale != object2.Required.Analog.GeneralScale)
            {
                Console.Error.WriteLine($"ANALOG:GEN_SCALE values are not compatible: {object1.Required.Analog.GeneralScale} | {object2.Required.Analog.GeneralScale}. Defaulting to General Scale from object 1.");
                object2.ChangeAnalogGeneralScale(object1.Required.Analog.GeneralScale);
            }
            // Sample per frame
            if (object1.Required.Analog.AnalogframePerFrame != object2.Required.Analog.AnalogframePerFrame)
            {
                throw new ArgumentException($"ANALOG sample per frames values are not compatible: {object1.Required.Analog.AnalogframePerFrame} | {object2.Required.Analog.AnalogframePerFrame} ");
            }

            // ROW CONCAT
            if (rowConcat)
            {

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

                    for (int idSample = 0; idSample < object1.Data.Points[idPartners.Item1].Residual.Length; idSample++)
                    {
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for (int idAxis = 0; idAxis < object1.Data.Points[idPartners.Item1].Point.GetLength(1); idAxis++)
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

                    newPointTrajectories.Add(new C3dPointTrajectory
                    {
                        Label = object1.Data.Points[idPartners.Item1].Label,
                        Description = object1.Data.Points[idPartners.Item1].Description,
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
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for (int idAxis = 0; idAxis < object2PointAxisNumber; idAxis++)
                        {
                            tempListPoint.Add(null);
                        }
                        newPointData.Add(tempListPoint.ToArray());

                        newResidualData.Add(null);

                        // Mask
                        List<bool> tempListMask = new List<bool>();
                        for (int idCamera = 0; idCamera < object2CameraNumber; idCamera++)
                        {
                            tempListMask.Add(false);
                        }
                        newCameraMaskData.Add(tempListMask.ToArray());

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
                foreach (int idSingle in singleTrajectoriesC3d2)
                {
                    List<float?[]> newPointData = new List<float?[]>();
                    List<float?> newResidualData = new List<float?>();
                    List<bool[]> newCameraMaskData = new List<bool[]>();

                    for (int idSample = 0; idSample < object2.Data.Points[idSingle].Residual.Length; idSample++)
                    {
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for (int idAxis = 0; idAxis < object2.Data.Points[idSingle].Point.GetLength(1); idAxis++)
                        {
                            tempListPoint.Add(object2.Data.Points[idSingle].Point[idSample, idAxis]);
                        }
                        newPointData.Add(tempListPoint.ToArray());

                        newResidualData.Add(null);

                        // Mask
                        List<bool> tempListMask = new List<bool>();
                        for (int idCamera = 0; idCamera < object2.Data.Points[idSingle].CameraMask.GetLength(1); idCamera++)
                        {
                            tempListMask.Add(object2.Data.Points[idSingle].CameraMask[idSample, idCamera]);
                        }
                        newCameraMaskData.Add(tempListMask.ToArray());
                    }

                    for (int idSample = 0; idSample < object2.Required.Point.Frames; idSample++)
                    {
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for (int idAxis = 0; idAxis < object2PointAxisNumber; idAxis++)
                        {
                            tempListPoint.Add(null);
                        }
                        newPointData.Add(tempListPoint.ToArray());

                        newResidualData.Add(null);

                        // Mask
                        List<bool> tempListMask = new List<bool>();
                        for (int idCamera = 0; idCamera < object2CameraNumber; idCamera++)
                        {
                            tempListMask.Add(false);
                        }
                        newCameraMaskData.Add(tempListMask.ToArray());

                    }

                    newPointTrajectories.Add(new C3dPointTrajectory
                    {
                        Label = object2.Data.Points[idSingle].Label,
                        Description = object2.Data.Points[idSingle].Description,
                        Point = newPointData.To2DArray(),
                        Residual = newResidualData.ToArray(),
                        CameraMask = newCameraMaskData.To2DArray()
                    });

                }

                // ANALOG
                // Associate per Labels, use the description of object1
                List<int> singleAnalogC3d1 = new List<int>();
                List<int> singleAnalogC3d2 = new List<int>();
                List<(int, int)> partnerAnalog = new List<(int, int)>();

                for (int label1 = 0; label1 < object1.Data.Analogs.Length; label1++)
                {
                    bool partnerFound = false;
                    for (int label2 = 0; label2 < object2.Data.Analogs.Length; label2++)
                    {
                        if (object2.Data.Analogs[label2].Label == object1.Data.Analogs[label1].Label)
                        {
                            partnerAnalog.Add((label1, label2));
                            partnerFound = true;
                            break;
                        }
                    }
                    if (!partnerFound)
                    {
                        singleAnalogC3d1.Add(label1);
                    }
                }
                for (int label2 = 0; label2 < object2.Data.Analogs.Length; label2++)
                {
                    bool partnerFound = false;
                    foreach ((int, int) partners in partnerAnalog)
                    {
                        if (partners.Item2 == label2)
                        {
                            partnerFound = true;
                            break;
                        }
                    }
                    if (!partnerFound)
                    {
                        singleAnalogC3d2.Add(label2);
                    }
                }
                // Check offset/Scale/Units/Bits.
                // Units and Bits is not recoverable.
                // offset and scale are switched to object1 values.
                // What if object1 analog empty, or object2 analog empty?

                // If somebody is missing labels association, add Offset valued sample
                List<C3dAnalogChannel> newAnalogChannels = new List<C3dAnalogChannel>();
                // Add partnered trajectories
                foreach ((int, int) idPartners in partnerTrajectories)
                {
                    List<float?[]> newPointData = new List<float?[]>();
                    List<bool[]> newCameraMaskData = new List<bool[]>();

                    for (int idSample = 0; idSample < object1.Data.Points[idPartners.Item1].Residual.Length; idSample++)
                    {
                        // Point data
                        List<float?> tempListPoint = new List<float?>();
                        for (int idAxis = 0; idAxis < object1.Data.Points[idPartners.Item1].Point.GetLength(1); idAxis++)
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


                }

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
