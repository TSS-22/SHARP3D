using SHARP3D.Data.DataEntity;
using SHARP3D.Parameter.DataEntity;
using SHARP3D.Utils;
using System.Reflection.Emit;
using static System.Formats.Asn1.AsnWriter;
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
                Points = GetPointDataFromFile(c3dFile.Data.Points, c3dFile.Point);
            }

            // Analogs
            if (c3dFile.Data.Points.Count != 0)
            {
                Analogs = GetAnalogDataFromFile(c3dFile.Data.Analogs, c3dFile.Analog);
            }

            // Forceplate
            if ((c3dFile.Forceplate.Used > 0) && (Analogs.Length!= 0))
            {
                Forceplates =  GetForcePlateDataFromFile(Analogs, c3dFile.Forceplate);
            }

            CleanForceplateInAnalog(c3dFile.Forceplate.Channel);
        }


        public (float?[,,], float?[,], bool[,,]) GetAllPointsData()
        {
            List<float?[,]> dataPoints = new List<float?[,]>();
            List<float?[]> dataResidual = new List<float?[]>();
            List<bool[,]> dataMask = new List<bool[,]>();

            foreach (C3dPointTrajectory trajectory in Points)
            {
                dataPoints.Add(trajectory.Point);
                dataResidual.Add(trajectory.Residual);
                dataMask.Add(trajectory.CameraMask);
            }

            return (dataPoints.To3DArray(), dataResidual.To2DArray(), dataMask.To3DArray());
        }

        public float[,] GetAllAnalogsData()
        {
            List<float[]> analogs = new List<float[]>();

            foreach (C3dAnalogChannel channel in Analogs)
            {
                analogs.Add(channel.Data);
            }

            return analogs.To2DArray();
        }

        public float[][,] GetAllForceplateData(bool applyCalibrationMatrix)
        {
            List<float[,]> allData = new List<float[,]>();

            foreach (C3dForceplate forcePlate in Forceplates) 
            {
                switch (applyCalibrationMatrix)
                {
                    case true:
                        allData.Add(forcePlate.GetAllData());
                        break;
                    default:
                        allData.Add(forcePlate.GetAllDataWithCalMat());
                        break;
                }
            }

            return allData.ToArray();
        }


        public void AddAnalogChannel(C3dAnalogChannel channelToAdd)
        {
            foreach (C3dAnalogChannel channel in Analogs)
            {
                if (channel.Label == channelToAdd.Label)
                {
                    throw new ArgumentException($"There is already an analog channel with label {channelToAdd.Label}. Labels must be uniques");
                }
            }
            List<C3dAnalogChannel> analogChannels = Analogs.ToList();
            analogChannels.Add(channelToAdd);
            Analogs = analogChannels.ToArray();
        }

        public void AddPointTrajectory(C3dPointTrajectory trajectoryToAdd)
        {
            foreach (C3dPointTrajectory trajectory in Points)
            {
                if (trajectory.Label == trajectoryToAdd.Label)
                {
                    throw new ArgumentException($"There is already a 3D point trajectory with label {trajectory.Label}. Labels must be uniques");
                }
            }
            List<C3dPointTrajectory> points = Points.ToList();
            points.Add(trajectoryToAdd);
            Points = points.ToArray();
        }

        public void AddForceplate(C3dForceplate forceplateToAdd)
        {
            foreach (C3dAnalogChannel channel in forceplateToAdd.Channels)
            {
                foreach (C3dAnalogChannel channel2 in forceplateToAdd.Channels)
                {
                    if (channel.Label == channel2.Label)
                    {
                        throw new ArgumentException($"There is already an analog channel with label {channel.Label}. Force plate channels Labels must be uniques");
                    }
                }
            }
            List<C3dForceplate> forceplates = Forceplates.ToList();
            forceplates.Add(forceplateToAdd);
            Forceplates = forceplates.ToArray();
        }

        public void DeleteForceplate(int[] idPlates)
        {
            List<C3dForceplate> newForceplate = new List<C3dForceplate> { };

            for (int idPlate = 0; idPlate < Forceplates.Length; idPlate++)
            {
                if (!idPlates.Contains(idPlate))
                {
                    newForceplate.Add(Forceplates[idPlate]);
                }
            }
            Forceplates = newForceplate.ToArray();
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
                            Point = points.To2DArray(),
                            Residual = residuals.ToArray(),
                            CameraMask = cameramasks.To2DArray()
                        });
                }
            }
            return c3DPoints.ToArray();
        }

        internal C3dAnalogChannel[] GetAnalogDataFromFile(
            List<float[][]> fileAnalogData,
            C3dFileParameterAnalog? fileParameterAnalog = null
            )
        {
            List<C3dAnalogChannel> c3dAnalogChannels = new List<C3dAnalogChannel>();
            if(fileAnalogData.Count() != 0)
            {
                for (int idChannel = 0; idChannel < fileAnalogData[0][0].Count(); idChannel++)
                {
                    // Data
                    List<float> dataToAdd = new List<float>();
                    for (int idFrame = 0; idFrame < fileAnalogData.Count(); idFrame++)
                    {
                        for (int idSample = 0; idSample < fileAnalogData[0].Count(); idSample++)
                        {
                            dataToAdd.Add(fileAnalogData[idFrame][idSample][idChannel]);
                        }
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
                        new C3dAnalogChannel (
                            fileParameterAnalog != null ? fileParameterAnalog.Bits : 12,
                            scaleToAdd,
                            descriptionToAdd,
                            labelToAdd,
                            offsetToAdd,
                            fileParameterAnalog != null ? fileParameterAnalog.Rate : fileAnalogData[0].Count() * fileAnalogData[1].Count(),
                            unitToAdd,
                            dataToAdd.ToArray()
                        ));
                }
            }
            return c3dAnalogChannels.ToArray();
        }

        internal C3dForceplate[] GetForcePlateDataFromFile(
            C3dAnalogChannel[] analogData,
            C3dFileParameterForceplate fileParameterForceplate
            )
        {
            List<C3dForceplate> c3dForceplates = new List<C3dForceplate>();
            int offsetPlateCalMat = 0;

            if (fileParameterForceplate.Channel.Count() != 0)
            {
                for (int idPlate = 0; idPlate < fileParameterForceplate.Used; idPlate++)
                {
                    C3dForceplate forceplateToAdd = new C3dForceplate();

                    List<C3dAnalogChannel> forceplateData = new List<C3dAnalogChannel>();
                    foreach (int idChannel in fileParameterForceplate.Channel[idPlate])
                    {
                        forceplateData.Add(analogData[idChannel]);
                    }

                    c3dForceplates.Add(
                        new C3dForceplate
                        {
                            CalibrationMatrix = fileParameterForceplate.CalibrationMatrix[idPlate],
                            Corners = fileParameterForceplate.Corners[idPlate],
                            Origin = fileParameterForceplate.Origin[idPlate],
                            Type = fileParameterForceplate.Type[idPlate],
                            Zero = fileParameterForceplate.Zero,
                            Channels = forceplateData.ToArray()
                        });
                }
                
            }
            return c3dForceplates.ToArray();
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
