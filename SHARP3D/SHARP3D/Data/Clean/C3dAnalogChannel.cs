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
        public float[][] Data = new float[][] { };

        public float[] GetData1D()
        {
            List<float> data1D = new List<float>();
            foreach (float[] frame in Data)
            {
                foreach (float sample in frame)
                {
                    data1D.Add(sample);
                }
            }
            return data1D.ToArray();
        }
    }
}
