using SHARP3D.Utils.Enum;

namespace SHARP3D.Data.Data
{
    public class C3dDataContext
    {
        public FileStream C3dStream { get; }
        public ProcessorType Processor { get; }
        public DataType DataTypeFile { get; }
        public int PointerDataSection { get; }
        public int FramesNumber { get; }
        public int MarkersPerFrame { get; }
        public float PointRate { get; }
        public float AnalogRate { get; }
        public int AnalogChannels { get; }

        public float PointScaleFactor { get; }
        public float AnalogScaleFactor { get; }

        public int AnalogSamplePerFrame { get; } 


        public C3dDataContext(
            FileStream c3dStream,
            ProcessorType processor,
            DataType dataTypeFile,
            int pointerDataSection,
            int framesNumber,
            int markersPerFrame,
            float pointRate,
            float analogRate,
            int analogChannels,
            float pointScaleFactor,
            float analogScaleFactor)
        {
            C3dStream = c3dStream;
            Processor = processor;
            DataTypeFile = dataTypeFile;
            PointerDataSection = pointerDataSection;
            FramesNumber = framesNumber;
            MarkersPerFrame = markersPerFrame;
            PointRate = pointRate;
            AnalogRate = analogRate;
            AnalogChannels = analogChannels;
            float tempAnalogSamplePerFrame = pointRate / analogRate;
            if (Math.Abs(tempAnalogSamplePerFrame - (int) tempAnalogSamplePerFrame) > 0)
            {
                throw new Exception("Rate incompatibility"); //TODO: make it cleaner
            }
            else
            {
                AnalogSamplePerFrame = ((int) tempAnalogSamplePerFrame) * AnalogChannels;
            }
            PointScaleFactor = pointScaleFactor;
            AnalogScaleFactor = analogScaleFactor;
        }
    }
}
