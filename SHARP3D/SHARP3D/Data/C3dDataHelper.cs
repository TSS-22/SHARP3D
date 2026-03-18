using SHARP3D.Data.Data;
using SHARP3D.Exceptions;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;


namespace SHARP3D.Data
{
    /// <summary>
    /// Provides helper methods for reading, parsing, and processing C3D file data.
    /// </summary>
    /// <remarks>
    /// This class contains utility methods to simplify common operations on C3D files,
    /// such as reading data frames, processing points and analogs, and validating data integrity.
    /// </remarks>
    public static class C3dDataHelper
    {
        /// <summary>
        /// Reads and parses C3D data from a file stream using the provided context.
        /// </summary>
        /// <param name="context">The <see cref="C3dDataContext"/> containing file stream and metadata.</param>
        /// <returns>A <see cref="C3dData"/> object containing parsed points and analogs.</returns>
        /// <exception cref="PointAndAnalogRateException">
        /// Thrown if the point rate and analog rate are not compatible.
        /// </exception>
        public static C3dData FromFileStream(C3dDataContext context) 
        {
            if ((context.AnalogRate % context.PointRate != 0) && (context.AnalogRate > context.PointRate))
            {
                throw new PointAndAnalogRateException("POINT:RATE and ANALOG:RATE don't match.");
            }
            if ((context.PointRate % context.AnalogRate != 0) && (context.PointRate > context.AnalogRate))
            {
                throw new PointAndAnalogRateException("POINT:RATE and ANALOG:RATE don't match.");
            }
            // TODO: Add the check for AnalogSamplePerFrame, total number of analog sample must be a mutliple of this. In the way the c3d file is done there is a better way to check for that I think

            context.C3dStream.Seek(context.PointerDataSection, SeekOrigin.Begin);

            return ReadAllData(context);


        }

        /// <summary>
        /// Reads all data frames from the C3D file and processes them into points and analogs.
        /// </summary>
        /// <param name="context">The <see cref="C3dDataContext"/> containing file stream and metadata.</param>
        /// <returns>A <see cref="C3dData"/> object containing parsed points and analogs.</returns>
        public static C3dData ReadAllData(C3dDataContext context)
        {
            List<C3dDataPoint[]> points = new List<C3dDataPoint[]>();
            List<float[][]> analogs = new List<float[][]>();
            
            for (int i = 0; i < context.FramesNumber; i++)
            {
                (C3dDataPoint[], float[][]) frame = ReadDataFrame(context);
                points.Add(frame.Item1);
                analogs.Add(frame.Item2);
            }
            return ProcessPointsAndAnalogsList(points, analogs);
        }

        /// <summary>
        /// Processes lists of points and analogs into a <see cref="C3dData"/> object.
        /// </summary>
        /// <param name="points">List of point arrays.</param>
        /// <param name="analogs">List of analog arrays.</param>
        /// <returns>A <see cref="C3dData"/> object containing the processed points and analogs.</returns>
        internal static C3dData ProcessPointsAndAnalogsList(List<C3dDataPoint[]> points, List<float[][]> analogs)
        {
            return new C3dData {
                Points = points,
                Analogs = analogs
            };
        }

        /// <summary>
        /// Reads a single data frame from the C3D file based on the data type.
        /// </summary>
        /// <param name="context">The <see cref="C3dDataContext"/> containing file stream and metadata.</param>
        /// <returns>A tuple containing arrays of points and analogs for the frame.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if the data type is not supported (neither INT16 nor FLOAT32).
        /// </exception>
        internal static (C3dDataPoint[], float[][]) ReadDataFrame(C3dDataContext context)
        {
            switch(context.DataTypeFile)
            {
                case DataType.INT16:
                    return ReadDataFrameInt16(context);

                case DataType.FLOAT32:
                    return ReadDataFrameFloat32(context);

                default:
                    throw new NotSupportedException("The C3D file data is neither stored in a supported format: INT16 or FLOAT32.");
            }
        }

        /// <summary>
        /// Reads a data frame with INT16 data type.
        /// </summary>
        /// <param name="context">The <see cref="C3dDataContext"/> containing file stream and metadata.</param>
        /// <returns>A tuple containing arrays of points and analogs for the frame.</returns>
        internal static (C3dDataPoint[], float[][]) ReadDataFrameInt16(C3dDataContext context) 
        {
            // Get POINTS
            List<C3dDataPoint> points = new List<C3dDataPoint>();
            List<float[]> analogs = new List<float[]>();

            for (int i = 0; i < context.MarkersPerFrame; i++)
            {
                List<float> pointValues = new List<float>();
                for (int j=0; j < 3; j++)
                { 
                    byte[] buffer = new byte[2];
                    context.C3dStream.ReadExactly(buffer);
                    pointValues.Add(C3dBytesConvertor.ToInt(buffer, context.Processor) * context.PointScaleFactor);
                }

                byte camAndSign = (byte)context.C3dStream.ReadByte();
                int residualInt = context.C3dStream.ReadByte();
                bool[] cameraMask = GetCameraMask(camAndSign);
                points.Add(new C3dDataPoint 
                {
                    Data = pointValues.ToArray(),
                    AverageResidual = residualInt * context.PointScaleFactor,
                    CameraMask = cameraMask,
                    Raw = IsRaw(camAndSign, residualInt),
                    Valid = IsValid(camAndSign, pointValues.ToArray(), cameraMask, context)
                });
            }
            // Get Analogs
            for (int i = 0; i < context.AnalogSamplePerFrame; i++)
            {
                float[] oneFullAnalogsSample = new float[context.AnalogChannels];
                for (int j = 0; j < context.AnalogChannels; j++)
                {
                    byte[] buffer = new byte[2];
                    context.C3dStream.ReadExactly(buffer);
                    oneFullAnalogsSample[j] = (C3dBytesConvertor.ToInt(buffer, context.Processor) - context.AnalogOffset) * context.AnalogChannelScaleFactor[j] * context.AnalogGeneralScaleFactor;
                }
                analogs.Add(oneFullAnalogsSample);
            }
            return (points.ToArray(), analogs.ToArray());
        }

        /// <summary>
        /// Reads a data frame with FLOAT32 data type.
        /// </summary>
        /// <param name="context">The <see cref="C3dDataContext"/> containing file stream and metadata.</param>
        /// <returns>A tuple containing arrays of points and analogs for the frame.</returns>
        internal static (C3dDataPoint[], float[][]) ReadDataFrameFloat32(C3dDataContext context) 
        {
            // Get POINTS
            List<C3dDataPoint> points = new List<C3dDataPoint>();
            //List<float> analogs = new List<float>();
            List<float[]> analogs = new List<float[]>();

            for (int i = 0; i < context.MarkersPerFrame; i++)
            {
                List<float> pointValues = new List<float>();
                for (int j = 0; j < 3; j++)
                {
                    byte[] buffer = new byte[4];
                    context.C3dStream.ReadExactly(buffer);
                    pointValues.Add(C3dBytesConvertor.ToFloat(buffer, context.Processor));
                }
                byte[] floatCamSignResidualBuffer = new byte[4];
                context.C3dStream.ReadExactly(floatCamSignResidualBuffer);
                // TODO: Handle out of range value but that shouldn't happen
                Int16 floatCamSignResidual = (Int16)C3dBytesConvertor.ToFloat(floatCamSignResidualBuffer, context.Processor);
                byte[] intCamSignResidual = BitConverter.GetBytes(floatCamSignResidual);
                byte camAndSign = intCamSignResidual[0];
                int residualInt = intCamSignResidual[1];

                bool[] cameraMask = GetCameraMask(camAndSign);

                points.Add(new C3dDataPoint
                {
                    Data = pointValues.ToArray(),
                    AverageResidual = residualInt * context.PointScaleFactor,
                    CameraMask = cameraMask,
                    Raw = IsRaw(camAndSign, residualInt),
                    Valid = IsValid(camAndSign, pointValues.ToArray(), cameraMask, context)
                });
            }
            // Get Analogs
            //List<float[]> analogValues = new List<float[]>();
            for (int i = 0; i < context.AnalogSamplePerFrame; i++)
            {
                float[] oneFullAnalogsSample = new float[context.AnalogChannels];
                for (int j = 0; j < context.AnalogChannels; j++)
                {
                    byte[] buffer = new byte[4];
                    context.C3dStream.ReadExactly(buffer);
                    oneFullAnalogsSample[j] = (C3dBytesConvertor.ToFloat(buffer, context.Processor) - context.AnalogOffset) * context.AnalogChannelScaleFactor[j] * context.AnalogGeneralScaleFactor;
                }
                //analogValues.Add(oneFullAnalogsSample);
                analogs.Add(oneFullAnalogsSample);
            }
            // Then I think I can just return the list<float> as array for analog and the List<C3dDataPoint> and get going.
            return (points.ToArray(), analogs.ToArray());
        }

        /// <summary>
        /// Determines if a point is raw or interpolated based on camera/sign byte and residual.
        /// </summary>
        /// <param name="camAndSign">The camera and sign byte.</param>
        /// <param name="residual">The residual value.</param>
        /// <returns>True if the point is raw; otherwise, false.</returns>
        internal static bool IsRaw(byte camAndSign, int residual)
        {
            if ((camAndSign == 0b00000001) || (residual == 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines if a point is valid based on camera/sign byte, point values, and camera mask.
        /// </summary>
        /// <param name="camAndSign">The camera and sign byte.</param>
        /// <param name="pointValue">The point values.</param>
        /// <param name="cameraMask">The camera mask.</param>
        /// <param name="context">The <see cref="C3dDataContext"/> containing file stream and metadata.</param>
        /// <returns>True if the point is valid; otherwise, false.</returns>
        /// <remarks>
        /// This value can't be trusted. Some people don't log it as specified in the C3D Guidelines. We tried our best to make it work reliably, but if you have any issue with a file, please contact us about it.
        /// </remarks>
        internal static bool IsValid(byte camAndSign, float[] pointValue, bool[] cameraMask, C3dDataContext context) 
        {
            // TODO: Isn't this shit show just that I forgot to take into account the differences between the processor ? I guess not because they do specify, byte 1, byte 2. But did they badly explain their shit again?
            //byte[] buffer = new byte[] { camAndSign, (byte)residual };
            //return C3dBytesConvertor.ToInt(buffer, context.Processor) < 0? true:false;
            //return ((camAndSign == 0b10000000) || (camAndSign == 0b00000000));
            bool theSupposedTestFromDocumentation =  (camAndSign & 0b10000000) == 0 ? true : false;
            bool apparentlyHowSomePeopleDecidedToInterpretInvalidMeasurement = !(pointValue.All(x => x == 0f) && cameraMask.Any(x => !x)); // If it True then that means the measurement is not valid, for some big brain companies.
            return theSupposedTestFromDocumentation && apparentlyHowSomePeopleDecidedToInterpretInvalidMeasurement;
            //bool signTest =  (camAndSign & 0b10000000) == 0 ? true : false;
            //bool cameraTest = cameraMask.Any(x => x);
            //return signTest && cameraTest; // Because some software don't save correctly the values correctly to tell if it is a valid or not measurement.
        }

        internal static bool[] GetCameraMask(byte camAndSign) 
        {
            bool[] cameraMask = new bool[7];
            for (int i = 0; i < 7; i++) // Loop through all 8 bits
            {
                int mask = 1 << i; // Create a mask for the i-th bit
                cameraMask[i] = (camAndSign & mask) != 0 ? true : false; // Check if the bit is set
            }
            return cameraMask;
        }
    }
}
