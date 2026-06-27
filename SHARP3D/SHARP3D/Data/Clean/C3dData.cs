using SHARP3D.Data.DataEntity;
using SHARP3D.Parameter.DataEntity;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SHARP3D.Data.Clean
{
    public class C3dData
    {
        public C3dPointTrajectory[] Points = new C3dPointTrajectory[] { };
        public C3dAnalogChannel[] Analogs = new C3dAnalogChannel[] { };
        public C3dForceplate[] Forceplates = new C3dForceplate[] { };

        public C3dData() { }
        public C3dData(C3dFile c3dFile)
        {
            // Points
            if (c3dFile.Data.Points.Count != 0)
            {
                Points = GetPointDataFromFile(c3dFile.Data.Points);
            }

            // Analogs
            if (c3dFile.Data.Points.Count != 0)
            {
                Analogs = GetAnalogDataFromFile(c3dFile.Data.Analogs);
            }

            // Forceplate
            if ((c3dFile.Forceplate.Used > 0) && (Analogs.Length!= 0))
            {
                Forceplates =  GetForcePlateDataFromFile(Analogs);
            }

            CleanForceplateInAnalog(c3dFile.Forceplate.Channel);
        }

        public (float?[,,], float?[,], bool[,,]) GetAllPointsData()
        {

            return (new float?[,,] { }, new float?[,] { }, new bool[,,] { });
        }

        public float[,] GetAllAnalogsData()
        {
            return new float[,] { };
        }

        public float[][,] GetAllForceplateData()
        {
            return new float[][,] { };
        }


        // TODO
        public void AddAnalogChannel()
        {

        }

        // TODO
        public void AddPointTrajectory()
        {

        }

        // TODO
        public void AddForceplate()
        {

        }

        // TODO
        public void DeleteForceplate(int[] idPlate)
        {

        }



        public void DeletePointTrajectories(string[] trajectoriesLabels)
        {
            List<int> trajectories = new List<int>();

            for (int idTraj = 0; idTraj < Points.Length; idTraj++)
            {
                foreach (string label in trajectoriesLabels)
                {
                    if (Points[idTraj].Label == label)
                    {
                        trajectories.Add(idTraj);
                        continue;
                    }
                }
            }

            DeletePointTrajectories(trajectories.ToArray());
        }

        public void DeletePointTrajectories(int[] trajectories)
        {
            List<C3dPointTrajectory> newPoints = new List<C3dPointTrajectory> { };

            for (int idTraj = 0; idTraj < Points.Length; idTraj++)
            {
                if (!trajectories.Contains(idTraj))
                {
                    newPoints.Add(Points[idTraj]);
                }
            }
            Points = newPoints.ToArray();
        }


        public void DeleteAnalogChannels(string[] channelLabels)
        {
            List<int> channels = new List<int>();

            for (int idChannel = 0; idChannel < Analogs.Length; idChannel++)
            {
                foreach (string label in channelLabels)
                {
                    if (Analogs[idChannel].Label == label)
                    {
                        channels.Add(idChannel);
                        continue;
                    }
                }
            }

            DeleteAnalogChannels(channels.ToArray());
        }

        public void DeleteAnalogChannels(int[] channels)
        {
            List<C3dAnalogChannel> newAnalogs = new List<C3dAnalogChannel> { };

            for (int idChannel = 0; idChannel < Analogs.Length; idChannel++)
            {
                if (!channels.Contains(idChannel))
                {
                    newAnalogs.Add(Analogs[idChannel]);
                }
            }
            Analogs = newAnalogs.ToArray();
        }

        

        internal C3dPointTrajectory[] GetPointDataFromFile(
            List<C3dFileDataPoint[]> filePointData,
            C3dFileParameterPoint? fileParameterPoint = null
            )
        {
            List<C3dPointTrajectory> c3DPoints = new List<C3dPointTrajectory>();
            if(filePointData.Count() != 0)
            {
                for (int idTraj = 0; idTraj < filePointData[0].Count(); idTraj++)
                {
                    List<float?[]> points = new List<float?[]>();
                    List<float?> residuals = new List<float?>();
                    List<bool[]> cameramasks = new List<bool[]>();

                    // Data
                    for (int idFrame = 0; idFrame < filePointData.Count; idFrame++)
                    {
                        // Point
                        List<float?> pointValues = new List<float?>();
                        foreach (float filePointValue in filePointData[idFrame][idTraj].Point)
                        {
                            pointValues.Add(filePointData[idFrame][idTraj].Valid ? filePointValue : null);
                        }
                        points.Add(pointValues.ToArray());

                        // Residual
                        residuals.Add(
                            filePointData[idFrame][idTraj].Raw ? filePointData[idFrame][idTraj].AverageResidual : null
                            );

                        //Camera Mask
                        cameramasks.Add(filePointData[idFrame][idTraj].CameraMask);

                    }

                    // Parameters
                    string labelToAdd = $"Trajectory {idTraj}";
                    string descriptionToAdd = $"No description provided for trajectory {idTraj}?";
                    if (fileParameterPoint != null)
                    {
                        // Label
                        try
                        {
                            labelToAdd = fileParameterPoint.Labels[idTraj];
                        }
                        catch (IndexOutOfRangeException) { }

                        // Description
                        try
                        {
                            descriptionToAdd = fileParameterPoint.Descriptions[idTraj];
                        }
                        catch (IndexOutOfRangeException) { }
                    }

                    c3DPoints.Add(
                        new C3dPointTrajectory
                        {
                            Label = labelToAdd,
                            Description = descriptionToAdd,
                            Point = ArrayUtils.To2DArray(points),
                            Residual = residuals.ToArray(),
                            CameraMask = ArrayUtils.To2DArray(cameramasks)
                        });
                }
            }
            return c3DPoints.ToArray();
        }

        // We are making the bet that going by frame is the right choice. Should be easier to put back in binaries maybe ?
        // I will make a function to get the analog in a simple 2D array
        internal C3dAnalogChannel[] GetAnalogDataFromFile(
            List<float[][]> fileAnalogData,
            C3dFileParameterAnalog? fileParameterAnalog = null
            )
        {
            List<C3dAnalogChannel> c3dAnalogChannels = new List<C3dAnalogChannel>();
            if(fileAnalogData.Count() != 0)
            {
                for (int idChannel = 0; idChannel < fileAnalogData[2].Count(); idChannel++)
                {
                    // Data
                    List<float[]> dataToAdd = new List<float[]>();
                    for (int idFrame = 0; idFrame < fileAnalogData[0].Count(); idFrame++)
                    {
                        List<float> sample = new List<float>();
                        for (int idSample = 0; idSample < fileAnalogData[1].Count(); idSample++)
                        {
                            sample.Add(fileAnalogData[idFrame][idSample][idChannel]);
                        }
                        dataToAdd.Add(sample.ToArray());
                    }
                    // Parameters
                    float scaleToAdd = 1;
                    string descriptionToAdd = $"No description provided for channel {idChannel}?";
                    string labelToAdd = $"Channel {idChannel}";
                    int offsetToAdd = 0;
                    string unitToAdd = "NA";
                    if (fileParameterAnalog != null)
                    {
                        // Scale
                        try
                        {
                            scaleToAdd = fileParameterAnalog.ChannelScale[idChannel];
                        }
                        catch (IndexOutOfRangeException) { }

                        // Description
                        try
                        {
                            descriptionToAdd = fileParameterAnalog.Descriptions[idChannel];
                        }
                        catch (IndexOutOfRangeException) { }

                        // Label
                        try
                        {
                            labelToAdd = fileParameterAnalog.Labels[idChannel];
                        }
                        catch (IndexOutOfRangeException) { }

                        // Offset
                        try
                        {
                            offsetToAdd = fileParameterAnalog.Offset[idChannel];
                        }
                        catch (IndexOutOfRangeException) { }

                        // Unit
                        try
                        {
                            unitToAdd = fileParameterAnalog.Units[idChannel];
                        }
                        catch (IndexOutOfRangeException) { }
                    }

                    c3dAnalogChannels.Add(
                        new C3dAnalogChannel
                        {
                            Bits = fileParameterAnalog != null ? fileParameterAnalog.Bits : 12,
                            Scale = scaleToAdd,
                            Description = descriptionToAdd,
                            Label = labelToAdd,
                            Offset = offsetToAdd,
                            Rate = fileParameterAnalog != null ? fileParameterAnalog.Rate : fileAnalogData[0].Count() * fileAnalogData[1].Count(),
                            Unit = unitToAdd,
                            Data = dataToAdd.ToArray()
                        });
                }
            }
            return c3dAnalogChannels.ToArray();
        }

        internal float[][,] GetForcePlateDataFromFile(float[,] analogData)
        {
            List<float[,]> totalForceplateData = new List<float[,]> { };
            // Get the data
            for (int idPlate = 0; idPlate < Required.Forceplate.Used; idPlate++)
            {
                // Get the type for calibration matrix
                // WARNING: It will go out of bound if I didn't do this properly
                float[] vecCalMat = new float[] { };
                float[,] calMat = new float[,] { };
                try
                {
                    vecCalMat = GetCalibrationMatrixVector(idPlate);
                    calMat = GetCalibrationMatrix(idPlate);
                }
                catch (Exception) { }


                // Compute the force plate offset
                int zeroFrameNb = Required.Forceplate.Zero.Item2 - Required.Forceplate.Zero.Item1;
                float[] zero = new float[Required.Forceplate.Channel[idPlate].Length];

                for (int idChannel = 0; idChannel < Required.Forceplate.Channel[idPlate].Length; idChannel++)
                {
                    float zeroData = 0.0f;
                    for (int idFrame = Required.Forceplate.Zero.Item1; idFrame < zeroFrameNb; idFrame++)
                    {
                        zeroData = analogData[idFrame, Required.Forceplate.Channel[idPlate][idChannel]];
                    }
                    zero[idChannel] = zeroData / zeroFrameNb;
                }

                // Initialize the force plate data array
                float[,] forceplateData = new float[analogData.GetLength(0), Required.Forceplate.Channel[idPlate].Length];
                // Populate the array
                for (int idFrame = 0; idFrame < analogData.GetLength(0); idFrame++)
                {
                    for (int idChannel = 0; idChannel < Required.Forceplate.Channel[idPlate].Length; idChannel++)
                    {
                        forceplateData[idFrame, idChannel] = analogData[idFrame, Required.Forceplate.Channel[idPlate][idChannel]] - zero[idChannel]; // Offset
                    }
                    // Apply calibration matrix if needs to
                    if ((Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_2) && vecCalMat.Length != 0)
                    {

                        for (int idChannel = 0; idChannel < Required.Forceplate.Channel[idPlate].Length; idChannel++)
                        {
                            forceplateData[idFrame, idChannel] = forceplateData[idFrame, idChannel] * vecCalMat[idChannel];
                        }
                    }
                    else if (Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_4 && calMat.Length != 0)
                    {
                        float[] tempforceplateDataRow = new float[forceplateData.GetLength(1)];

                        for (int idChannel = 0; idChannel < Required.Forceplate.Channel[idPlate].Length; idChannel++)
                        {
                            tempforceplateDataRow[idChannel] = forceplateData[idFrame, idChannel];
                        }
                        tempforceplateDataRow = ArrayUtils.VecMatMultiplication(tempforceplateDataRow, calMat);
                        for (int idChannel = 0; idChannel < Required.Forceplate.Channel[idPlate].Length; idChannel++)
                        {
                            forceplateData[idFrame, idChannel] = tempforceplateDataRow[idChannel];
                        }
                    }
                    else
                    {
                        // Need a better place for this. Is it redundant ?
                        //Console.WriteLine($"Plateform of type {Required.Forceplate.Type} doesn't support calibration matrix.");

                    }
                }

                totalForceplateData.Add(forceplateData);
            }
            return totalForceplateData.ToArray();
        }

        public void CleanForceplateInAnalog(int[][] forcePlateChannels)
        {
            // FORCE_PLATFORM clean up
            List<int> channelsToDelete = new List<int>();
            foreach (int[] channels in forcePlateChannels)
            {
                foreach (int channel in channels)
                {
                    channelsToDelete.Add(channel);
                }
            }
            DeleteAnalogChannels(channelsToDelete.ToArray());
        }

    }
}
