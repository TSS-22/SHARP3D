using SHARP3D.Data.Data;
using SHARP3D.Exceptions;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Drawing;

namespace SHARP3D.Data
{
    public static class C3dDataHelper
    {
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

        internal static C3dData ProcessPointsAndAnalogsList(List<C3dDataPoint[]> points, List<float[][]> analogs)
        {
            return new C3dData {
                Points = points,
                Analogs = analogs
            };
        }

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

        internal static (C3dDataPoint[], float[][]) ReadDataFrameInt16(C3dDataContext context) 
        {
            // Get POINTS
            List<C3dDataPoint> points = new List<C3dDataPoint>();
            //List<float> analogs = new List<float>();
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
            //List<float[]> analogValues = new List<float[]>();
            for (int i = 0; i < context.AnalogSamplePerFrame; i++)
            {
                float[] oneFullAnalogsSample = new float[context.AnalogChannels];
                for (int j = 0; j < context.AnalogChannels; j++)
                {
                    byte[] buffer = new byte[2];
                    context.C3dStream.ReadExactly(buffer);
                    oneFullAnalogsSample[j] = (C3dBytesConvertor.ToInt(buffer, context.Processor) - context.AnalogOffset) * context.AnalogChannelScaleFactor[j] * context.AnalogGeneralScaleFactor;
                }
                //analogValues.Add(oneFullAnalogsSample);
                analogs.Add(oneFullAnalogsSample);
            }
            // Then I think I can just return the list<float> as array for analog and the List<C3dDataPoint> and get going.
            return (points.ToArray(), analogs.ToArray());
        }
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
                Int32 floatCamSignResidual = (Int32)C3dBytesConvertor.ToFloat(floatCamSignResidualBuffer, context.Processor);
                byte[] intCamSignResidual = BitConverter.GetBytes(floatCamSignResidual);
                byte camAndSign;
                int residualInt;
                if (BitConverter.IsLittleEndian)
                {
                    camAndSign = intCamSignResidual[0];  
                    residualInt = intCamSignResidual[1];
                } else
                {
                    camAndSign = intCamSignResidual[3];
                    residualInt = intCamSignResidual[2];
                }

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

        internal static bool IsValid(byte camAndSign, float[] pointValue, bool[] cameraMask, C3dDataContext context) 
        {
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
