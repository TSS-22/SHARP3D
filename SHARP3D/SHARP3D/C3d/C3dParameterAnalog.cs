namespace SHARP3D.C3d
{
    public struct C3dParameterAnalog
    {
        public int Bits { get; set; }
        public string[] Descriptions;
        public float GeneralScale;
        public string[] Labels;
        public int[] Offset;
        public float Rate;
        public float[] ChannelScale;
        public string[] Units;
        public int Used;
    }

}