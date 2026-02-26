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
        public int AnalogPerFrame { get; }

        public C3dDataContext(
            FileStream c3dStream,
            ProcessorType processor,
            DataType dataTypeFile,
            int pointerDataSection,
            int framesNumber,
            int markersPerFrame,
            float pointRate,
            float analogRate,
            int analogPerFrame)
        {
            C3dStream = c3dStream;
            Processor = processor;
            DataTypeFile = dataTypeFile;
            PointerDataSection = pointerDataSection;
            FramesNumber = framesNumber;
            MarkersPerFrame = markersPerFrame;
            PointRate = pointRate;
            AnalogRate = analogRate;
            AnalogPerFrame = analogPerFrame;
        }
    }
}
