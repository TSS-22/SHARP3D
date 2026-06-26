using SHARP3D.Data.DataEntity;
using SHARP3D.Exceptions;
using SHARP3D.Parameter.DataEntity;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;


namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;
        public C3dParameterPoint RequiredPoint = new C3dParameterPoint();
        public C3dParameterAnalog RequiredAnalog = new C3dParameterAnalog();
        public C3dParameterForceplate RequiredForceplate = new C3dParameterForceplate();

        public C3dParameterSection Parameters = new C3dParameterSection();

        public C3dData Data = new C3dData(); 

        public C3d(string filePath)
        {
            C3dFile c3DFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3DFile.FilePath;
            
            RequiredPoint = c3DFile.Point;
            RequiredAnalog = c3DFile.Analog;
            RequiredForceplate = c3DFile.Forceplate;

            Parameters = new C3dParameterSection(c3DFile.Parameters);

            Data = GetDataFromFile(c3DFile.Data);

            CleanUpParameters();

        }

        public C3d() { }

        internal void CleanUpParameters()
        {
            // Discard from "Parameters" the required parameters
            foreach (KeyValuePair<string, string[]> group in Sharp3dConstants.ParameterToDiscardFromC3dFileToC3d)
            {
                foreach (string parameter in group.Value)
                {
                    try
                    {
                        Parameters.DeleteParameter(group.Key, parameter);
                    }
                    catch (ArgumentException) { }
                    
                }
            }
            
            // FORCE_PLATFORM clean up
            List<int> channelsToDelete = new List<int>();
            foreach (int[] channels in RequiredForceplate.Channel)
            {
                foreach(int channel in channels)
                {
                    channelsToDelete.Add(channel);
                }
            }
            DeleteAnalogChannels(channelsToDelete.ToArray());

        }

        // TODO
        internal void AddAnalogChannel()
        {

        }

        // TODO
        internal void AddPointTrajectory()
        {

        }

        internal void DeletePointTrajectories(string[] trajectoriesLabels)
        {
            List<int> trajectories = new List<int>();

            for (int idTraj = 0; idTraj < RequiredPoint.Labels.Length; idTraj++)
            {
                foreach (string label in trajectoriesLabels)
                {
                    if (RequiredAnalog.Labels[idTraj] == label)
                    {
                        trajectories.Add(idTraj);
                        continue;
                    }
                }
            }

            DeletePointTrajectories(trajectories.ToArray());
        }

        internal void DeletePointTrajectories(int[] trajectories)
        {
            // Data side
            float?[,,] newDataPoint = new float?[,,] { };
            if ((RequiredPoint.Used - trajectories.Length != 0))
            {
                newDataPoint = new float?[RequiredPoint.Frames, RequiredPoint.Used - trajectories.Length, Data.Point.GetLength(2)];
            }

            float?[,] newResidual = new float?[RequiredPoint.Frames, RequiredPoint.Used - trajectories.Length];
            bool[,,] newCameramask = new bool[RequiredPoint.Frames, RequiredPoint.Used - trajectories.Length, Data.CameraMask.GetLength(2)];
            // Parameter side
            string[] newDescriptions = new string[RequiredPoint.Used - trajectories.Length];
            string[] newLabels = new string[RequiredPoint.Used - trajectories.Length];

            int offsetTraj = 0; // To work the channel taken out during the populating phase
            if (Data.Analog != null)
            {
                float maxPointValue = 0;
                for (int idTraj = 0; idTraj < RequiredAnalog.Used; idTraj++)
                {
                    if (trajectories.Contains(idTraj))
                    {
                        offsetTraj++;
                        continue;
                    }
                    else
                    {
                        for (int idFrame = 0; idFrame < RequiredAnalog.TotalSamples; idFrame++)
                        {
                            // Point
                            for(int idPoint = 0; idPoint < Data.Point.GetLength(2); idPoint++) 
                            {
                                newDataPoint[idFrame, idTraj - offsetTraj, idPoint] = Data.Point[idFrame, idTraj, idPoint];
                                if(Data.Point[idFrame, idTraj, idPoint] != null) 
                                {
                                    if (maxPointValue > Math.Abs((float)Data.Point[idFrame, idTraj, idPoint]))
                                    {
                                        maxPointValue = Math.Abs((float)Data.Point[idFrame, idTraj, idPoint]);
                                    }
                                }
                                
                            }
                            // Residual
                            newResidual[idFrame, idTraj - offsetTraj] = Data.Residual[idFrame, idTraj];
                            // Camera mask
                            for (int idCamera = 0; idCamera < Data.Point.GetLength(2); idCamera++)
                            {
                                newCameramask[idFrame, idTraj - offsetTraj, idCamera] = Data.CameraMask[idFrame, idTraj, idCamera];
                            }
                            // Parameters
                            newDescriptions[idTraj - offsetTraj] = RequiredPoint.Descriptions[idTraj];
                            newLabels[idTraj - offsetTraj] = RequiredPoint.Labels[idTraj];

                        }
                    }

                    Data.Point = newDataPoint;
                    Data.Residual = newResidual;
                    Data.CameraMask = newCameramask;

                    RequiredPoint.Descriptions = newDescriptions;
                    RequiredPoint.Labels = newLabels;
                    RequiredPoint.Scale = maxPointValue / 32000;
                    RequiredPoint.Used = newDataPoint.GetLength(1);
                }
            }
            else
            {
                Console.WriteLine("Can't delete trajectories: there is no Point data.");
            }
        }


        internal void DeleteAnalogChannels(string[] channelLabels)
        {
            List<int> channels = new List<int>();

            for(int idChannel = 0; idChannel < RequiredAnalog.Labels.Length; idChannel++)
            {
                foreach (string label in channelLabels)
                {
                    if (RequiredAnalog.Labels[idChannel] == label)
                    {
                        channels.Add(idChannel);
                        continue;
                    }
                }
            }

            DeleteAnalogChannels(channels.ToArray());
        }

        internal void DeleteAnalogChannels(int[] channels)
        {
            float[,] newDataAnalog = new float[,] { };
            if (RequiredAnalog.Used - channels.Length != 0)
            {
                newDataAnalog = new float[RequiredAnalog.TotalSamples, RequiredAnalog.Used - channels.Length];
            }


            float[] newChannelScale = new float[RequiredAnalog.Used - channels.Length];
            string[] newDescriptions = new string[RequiredAnalog.Used - channels.Length];
            string[] newLabelsAnalog = new string[RequiredAnalog.Used - channels.Length];
            int[] newOffset = new int[RequiredAnalog.Used - channels.Length];
            string[] newUnits = new string[RequiredAnalog.Used - channels.Length];
            
            int offsetChannel = 0; // To work the channel taken out during the populating phase

            if(Data.Analog != null)
            {
                // Get data and Labels
                for (int idChannel = 0; idChannel < RequiredAnalog.Used; idChannel++)
                {
                    if (channels.Contains(idChannel))
                    {
                        offsetChannel++;
                        continue;
                    }
                    else
                    {
                        for (int idSample=0; idSample < RequiredAnalog.TotalSamples; idSample++)
                        {
                    
                            newDataAnalog[idSample, idChannel - offsetChannel] = Data.Analog[idSample, idChannel];

                            newChannelScale[idChannel - offsetChannel] = RequiredAnalog.ChannelScale[idChannel];
                            newDescriptions[idChannel - offsetChannel] = RequiredAnalog.Descriptions[idChannel];
                            newLabelsAnalog[idChannel - offsetChannel] = RequiredAnalog.Labels[idChannel];
                            newOffset[idChannel - offsetChannel] = RequiredAnalog.Offset[idChannel];
                            newUnits[idChannel - offsetChannel] = RequiredAnalog.Units[idChannel];
                        }
                    }
                }
                Data.Analog = newDataAnalog;

                RequiredAnalog.ChannelScale = newChannelScale;
                RequiredAnalog.Descriptions = newDescriptions;
                RequiredAnalog.Labels = newLabelsAnalog;
                RequiredAnalog.Offset = newOffset;
                RequiredAnalog.Units = newUnits;

                RequiredAnalog.Used = newDataAnalog.GetLength(1); // In case some of the channels where not found.
            }
            else
            {
                Console.WriteLine("Can't delete channels: there is no Analog data.");
            }
        }


        internal C3dData GetDataFromFile(C3dFileData fileData) 
        {
            C3dData data = new C3dData();
            if(fileData.Points.Count != 0)
            {
                (data.Point, data.Residual, data.CameraMask) = GetPointDataFromFile(fileData.Points);
            }

            data.Analog = fileData.Analogs.Count != 0 ? GetAnalogDataFromFile(fileData.Analogs) : null;
            data.ForcePlate = (RequiredForceplate.Used > 0 ) && (data.Analog != null) ? GetForcePlateDataFromFile(data.Analog) : null;

            // Discard force_platform data from analog before returning #177
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
                    if (filePointData[idFrame][idTraj].Valid != false)
                    {
                        for (int idPoint=0; idPoint < nbPoint; idPoint++) 
                        {
                            point[idFrame, idTraj, idPoint] = filePointData[idFrame][idTraj].Point[idPoint];
                        }
                    }

                    // Residual populating
                    if (filePointData[idFrame][idTraj].Raw!=false)
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
        internal float[,] GetAnalogDataFromFile(List<float[][]> fileAnalogData)
        {
            // This is the number of frame for the analog array creation
            int nbFrame = fileAnalogData.Count * RequiredAnalog.SamplesPerFrame;
            float[,] analog = new float[nbFrame, RequiredAnalog.Used];

            for (int idFrame = 0; idFrame < fileAnalogData.Count; idFrame++)
            {
                for (int idSample = 0; idSample < RequiredAnalog.SamplesPerFrame; idSample++)
                {
                    for (int idChannel = 0; idChannel < RequiredAnalog.Used; idChannel++)
                    {
                        analog[idFrame * 4 + idSample, idChannel] = fileAnalogData[idFrame][idSample][idChannel];
                    }
                }
            }
            
            return (analog);
        }

        internal float[][,] GetForcePlateDataFromFile(float[,] analogData)
        {
            List<float[,]> totalForceplateData = new List<float[,]> { };
            // Get the data
            for(int idPlate=0; idPlate < RequiredForceplate.Used; idPlate++)
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
                int zeroFrameNb = RequiredForceplate.Zero.Item2 - RequiredForceplate.Zero.Item1;
                float[] zero = new float[RequiredForceplate.Channel[idPlate].Length];

                for (int idChannel = 0; idChannel < RequiredForceplate.Channel[idPlate].Length; idChannel++)
                {
                    float zeroData = 0.0f;
                    for (int idFrame = RequiredForceplate.Zero.Item1; idFrame < zeroFrameNb; idFrame++)
                    {
                        zeroData = analogData[idFrame, RequiredForceplate.Channel[idPlate][idChannel]];
                    }
                    zero[idChannel] = zeroData / zeroFrameNb;
                }
                
                // Initialize the force plate data array
                float[,] forceplateData = new float[analogData.GetLength(0), RequiredForceplate.Channel[idPlate].Length];
                // Populate the array
                for(int idFrame=0; idFrame < analogData.GetLength(0); idFrame++)
                {
                    for (int idChannel = 0; idChannel < RequiredForceplate.Channel[idPlate].Length; idChannel++)
                    {
                        forceplateData[idFrame, idChannel] = analogData[idFrame, RequiredForceplate.Channel[idPlate][idChannel]] - zero[idChannel]; // Offset
                    }
                    // Apply calibration matrix if needs to
                    if ((RequiredForceplate.Type[idPlate] == ForceplateType.TYPE_2) && vecCalMat.Length != 0)
                    {

                        for (int idChannel = 0; idChannel < RequiredForceplate.Channel[idPlate].Length; idChannel++)
                        {
                            forceplateData[idFrame, idChannel] = forceplateData[idFrame, idChannel] * vecCalMat[idChannel];
                        }
                    }
                    else if (RequiredForceplate.Type[idPlate] == ForceplateType.TYPE_4 && calMat.Length != 0)
                    {
                        float[] tempforceplateDataRow = new float[forceplateData.GetLength(1)];

                        for (int idChannel = 0; idChannel < RequiredForceplate.Channel[idPlate].Length; idChannel++)
                        {
                            tempforceplateDataRow[idChannel] = forceplateData[idFrame, idChannel];
                        }
                        tempforceplateDataRow = ArrayUtils.VecMatMultiplication(tempforceplateDataRow, calMat);
                        for (int idChannel = 0; idChannel < RequiredForceplate.Channel[idPlate].Length; idChannel++)
                        {
                            forceplateData[idFrame, idChannel] = tempforceplateDataRow[idChannel];
                        }
                    }
                    else
                    {
                        // Need a better place for this. Is it redundant ?
                        //Console.WriteLine($"Plateform of type {RequiredForceplate.Type} doesn't support calibration matrix.");

                    }
                }

                totalForceplateData.Add(forceplateData);
            }
            return totalForceplateData.ToArray();
        }

        internal float[] GetCalibrationMatrixVector(int idPlate)
        {
            try
            {
                C3dParameter calibrationMatrixParameter = Parameters.GetParameter("force_platform", "cal_matrix");

                float[] calibrationVector = Enumerable.Repeat(1.0f, calibrationMatrixParameter.Dimensions[0]).ToArray();
                if ((RequiredForceplate.Type[idPlate] == ForceplateType.TYPE_2) || (RequiredForceplate.Type[idPlate] == ForceplateType.TYPE_4))
                {
                    for(int i=0; i < calibrationVector.Length; i++)
                    {
                        try
                        {
                            calibrationVector[i] = calibrationMatrixParameter.Data.GetValue(i, i, idPlate) as float? ?? throw new NullReferenceException();
                        }
                        catch(NullReferenceException) 
                        {
                            Console.Error.WriteLine($"WARNING: Force plate id {idPlate} calibration matrix is null at index: [{i},{i}]");
                        }
                    }
                }
                else
                {
                    throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {RequiredForceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                }
                return calibrationVector;
            }
            catch (ArgumentException) { throw; }
            catch (NoCalibrationMatrixForForceplateType) { throw; }
        }

        internal float[,] GetCalibrationMatrix(int idPlate)
        {
            try
            {
                C3dParameter calibrationMatrixParameter = Parameters.GetParameter("force_platform", "cal_matrix");

                float[,] calibrationMatrix = new float[calibrationMatrixParameter.Dimensions[1], calibrationMatrixParameter.Dimensions[0]];
                if ((RequiredForceplate.Type[idPlate] == ForceplateType.TYPE_2) || (RequiredForceplate.Type[idPlate] == ForceplateType.TYPE_4))
                {
                    for (int i = 0; i < calibrationMatrixParameter.Dimensions[1]; i++)
                    {
                        for (int j = 0; j < calibrationMatrixParameter.Dimensions[0]; j++)
                        {
                            calibrationMatrix[i, j] = 1.0f;
                        }
                    }

                    if (RequiredForceplate.Type[idPlate] == ForceplateType.TYPE_2)
                    {
                        for (int col = 0; col < calibrationMatrix.GetLength(0); col++)
                        {
                            for (int row = 0; row < calibrationMatrix.GetLength(1); row++)
                            {
                                try
                                {
                                    calibrationMatrix[row, col] = calibrationMatrixParameter.Data.GetValue(col, row, idPlate) as float? ?? throw new NullReferenceException();
                                }
                                catch (NullReferenceException)
                                {
                                    Console.Error.WriteLine($"WARNING: Force plate id {idPlate} calibration matrix is null at index: [{col},{row}]");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {RequiredForceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                    }
                    return calibrationMatrix;
                }
                else 
                {
                    throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {RequiredForceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                }
                
            }
            catch (ArgumentException) { throw; }
            catch (NoCalibrationMatrixForForceplateType) { throw; }
        }
    }
}
