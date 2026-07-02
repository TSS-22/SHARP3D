namespace SHARP3D.Data.Clean
{
    public class C3dAnalogChannel
    {
        public int Bits = 12; // In case we have differents bits resolution. Wont be useful for C3D but will be for our file type.
        public float Scale = 1.0f;
        public string Description = "No description provided";
        public string Label = "Unkown";
        public int Offset = 0;
        public float Rate = 0.0f;
        public string Unit;
        public float[] Data = new float[] { };

        public float Descale(float scaledValue, float generalScale=1.0f)
        {
            return (scaledValue/(generalScale * Scale)) + Offset;
        }

        public float[] DescaledData(float[] scaledData, float generalScale=1.0f)
        {
            float[] descaledData = new float[scaledData.Length];
            for (int i = 0; i < scaledData.Length; i++)
            {
                descaledData[i] = Descale(scaledData[i], generalScale);
            }
            return descaledData;
        }
    }
}
