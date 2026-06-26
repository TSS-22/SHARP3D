using SHARP3D.Data.Clean;
using SHARP3D.Data.DataEntity;
using SHARP3D.Exceptions;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;


namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;
        public RequiredParameters Required = new RequiredParameters();

        public C3dParameterSection Parameters = new C3dParameterSection();

        public C3dData Data = new C3dData(); 

        public C3d(string filePath)
        {
            C3dFile c3DFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3DFile.FilePath;
            
            Required.Point = c3DFile.Point;
            Required.Analog = c3DFile.Analog;
            Required.Forceplate = c3DFile.Forceplate;

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
            foreach (int[] channels in Required.Forceplate.Channel)
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

        // TODO
        internal void AddForceplate()
        {

        }

        // TODO
        internal void DeleteForceplate(int[] idPlate)
        {

        }

        

        internal void DeletePointTrajectories(string[] trajectoriesLabels)
        {
            List<int> trajectories = new List<int>();

            for (int idTraj = 0; idTraj < Data.Points.Length; idTraj++)
            {
                foreach (string label in trajectoriesLabels)
                {
                    if (Data.Points[idTraj].Label == label)
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
            List<C3dPointTrajectory> newPoints = new List<C3dPointTrajectory> { };
            
            int offsetTraj = 0; // To work the channel taken out during the populating phase
            for (int idTraj = 0; idTraj < Data.Points.Length; idTraj++)
            {
                if (!trajectories.Contains(idTraj))
                {
                    newPoints.Add(Data.Points[idTraj]);
                }
            }
            Data.Points = newPoints.ToArray();
        }


        internal void DeleteAnalogChannels(string[] channelLabels)
        {
            List<int> channels = new List<int>();

            for(int idChannel = 0; idChannel < Data.Analogs.Length; idChannel++)
            {
                foreach (string label in channelLabels)
                {
                    if (Data.Analogs[idChannel].Label == label)
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
            List<C3dAnalogChannel> newAnalogs = new List<C3dAnalogChannel> { };

            int offsetTraj = 0; // To work the channel taken out during the populating phase
            for (int idChannel = 0; idChannel < Data.Points.Length; idChannel++)
            {
                if (!channels.Contains(idChannel))
                {
                    newAnalogs.Add(Data.Analogs[idChannel]);
                }
            }
            Data.Analogs = newAnalogs.ToArray();
        }


        internal C3dData GetDataFromFile(C3dFileData fileData) 
        {
            C3dData data = new C3dData();
            if(fileData.Points.Count != 0)
            {
                (data.Point, data.Residual, data.CameraMask) = GetPointDataFromFile(fileData.Points);
            }

            data.Analog = fileData.Analogs.Count != 0 ? GetAnalogDataFromFile(fileData.Analogs) : null;
            data.ForcePlate = (Required.Forceplate.Used > 0 ) && (data.Analog != null) ? GetForcePlateDataFromFile(data.Analog) : null;

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
            int nbFrame = fileAnalogData.Count * Required.Analog.SamplesPerFrame;
            float[,] analog = new float[nbFrame, Required.Analog.Used];

            for (int idFrame = 0; idFrame < fileAnalogData.Count; idFrame++)
            {
                for (int idSample = 0; idSample < Required.Analog.SamplesPerFrame; idSample++)
                {
                    for (int idChannel = 0; idChannel < Required.Analog.Used; idChannel++)
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
            for(int idPlate=0; idPlate < Required.Forceplate.Used; idPlate++)
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
                for(int idFrame=0; idFrame < analogData.GetLength(0); idFrame++)
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

        internal float[] GetCalibrationMatrixVector(int idPlate)
        {
            try
            {
                C3dParameter calibrationMatrixParameter = Parameters.GetParameter("force_platform", "cal_matrix");

                float[] calibrationVector = Enumerable.Repeat(1.0f, calibrationMatrixParameter.Dimensions[0]).ToArray();
                if ((Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_2) || (Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_4))
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
                    throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {Required.Forceplate.Type[idPlate]} and therefore don't have calibration matrix.");

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
                if ((Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_2) || (Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_4))
                {
                    for (int i = 0; i < calibrationMatrixParameter.Dimensions[1]; i++)
                    {
                        for (int j = 0; j < calibrationMatrixParameter.Dimensions[0]; j++)
                        {
                            calibrationMatrix[i, j] = 1.0f;
                        }
                    }

                    if (Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_2)
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
                        throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {Required.Forceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                    }
                    return calibrationMatrix;
                }
                else 
                {
                    throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {Required.Forceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                }
                
            }
            catch (ArgumentException) { throw; }
            catch (NoCalibrationMatrixForForceplateType) { throw; }
        }
    }
}
