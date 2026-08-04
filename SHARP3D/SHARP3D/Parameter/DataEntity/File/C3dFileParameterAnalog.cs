namespace SHARP3D.Parameter.DataEntity
{
    public class C3dFileParameterAnalog
    {
        public int Bits { get; set; } = 12;
        public float[] ChannelScale = new float[] { };
        public string[] Descriptions = new string[] { };
        public float GeneralScale = 1;
        public string[] Labels = new string[] { };
        public int[] Offset = new int[] { } ;
        public float Rate = 0;
        public int AnalogframePerFrame = 0;
        public int TotalSamples = 0;
        public string[] Units = new string[] { };
        public int Used = 0;
    }

}