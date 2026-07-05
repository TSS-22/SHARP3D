using System.Numerics;

namespace SHARP3D.Data.Clean
{
    public class C3dAnalogChannel
    {
        public int Bits = 12; // In case we have differents bits resolution. Wont be useful for C3D but will be for our file type.
        private float _scale = 1.0f;
        public float Scale
        {
            get => _scale;
            set
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    Data[i] = Data[i] / _scale * value;
                }
                _scale = value;
            }
        }
        public string Description = "No description provided";
        public string Label = "Unkown";
        private int _offset = 0;
        public int Offset
        {
            get => _offset;
            private set => _offset = value;
        }
        public float Rate = 0.0f; // At the moment it might be useless as the rate is infered from the data length in comparison with the Point.Data length
        private string _units;
        public string Units
        {
            get => _units;
            private set => _units = value;
        }
        public float[] Data = new float[] { };

        public C3dAnalogChannel(int bits, float scale, string description, string label, int offset, float rate, string units, float[] data)
        {
            Bits = bits;
            Scale = scale;
            Description = description;
            Label = label;
            Offset = offset;
            Rate = rate;
            Units = units;
            Data = data;
        }

        public float DescaleData(float scaledValue, float generalScale=1.0f)
        {
            return (scaledValue/(generalScale * Scale)) + (float)Offset;
        }

        public float[] GetAllDescaledData(float generalScale=1.0f)
        {
            float[] descaledData = new float[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                descaledData[i] = DescaleData(Data[i], generalScale);
            }
            return descaledData;
        }

        public float ScaleData(float rawValue, float generalScale = 1.0f) 
        {
            return (rawValue - (float)Offset) * Scale * generalScale;
        }

        public float[] ScaleAllData(float[] rawData, float generalScale = 1.0f)
        {
            for(int i=0; i<rawData.Length;i++)
            {
                rawData[i] = ScaleData(rawData[i], generalScale);
            }
            return rawData;
        }

        public void ChangeUnit(string newUnits, float factor)
        {
            Units = newUnits;
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = Data[i] * factor;
            }
        }

        public void ChangeOffset(int newOffset, float generalScale = 1.0f)
        {
            float[] descaledData = GetAllDescaledData(generalScale);
            Offset = newOffset;
            Data = ScaleAllData(descaledData, generalScale);
        }

    }
}
