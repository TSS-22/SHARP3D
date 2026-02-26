using SHARP3D.Data.Data;
using SHARP3D.Exceptions;
using SHARP3D.Utils.Enum;

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

            context.C3dStream.Seek((context.PointerDataSection - 1) * 512, SeekOrigin.Begin);

            return ReadAllData(context);


        }


        public static C3dData ReadAllData(C3dDataContext context)
        {
            List<C3dDataFramePoint> points = new List<C3dDataFramePoint>();
            List<C3dDataFrameAnalog> analogs = new List<C3dDataFrameAnalog>();
            
            for (int i = 0; i < context.FramesNumber; i++)
            {
                (C3dDataFramePoint, C3dDataFrameAnalog) frame = ReadDataFrame(context);
                points.Add(frame.Item1);
                analogs.Add(frame.Item2);
            }
            return ProcessPointsAndAnalogsList(points, analogs);
        }

        internal static C3dData ProcessPointsAndAnalogsList(List<C3dDataFramePoint> points, List<C3dDataFrameAnalog> analogs)
        {
            return new C3dData();
        }

        internal static (C3dDataFramePoint, C3dDataFrameAnalog) ReadDataFrame(C3dDataContext context)
        {
            switch(context.DataTypeFile)
            {
                case DataType.INT16:
                    return ReadDataFrameInt16(context);

                case DataType.FLOAT32:
                    return ReadDataFrameInt16(context);

                default:
                    throw new NotSupportedException("The C3D file data is neither stored in a supported format: INT16 or FLOAT32.");
            }
        }

        internal static (C3dDataFramePoint, C3dDataFrameAnalog) ReadDataFrameInt16(C3dDataContext context) { }
        internal static (C3dDataFramePoint, C3dDataFrameAnalog) ReadDataFrameFloat32(C3dDataContext context) { }
    }
}
