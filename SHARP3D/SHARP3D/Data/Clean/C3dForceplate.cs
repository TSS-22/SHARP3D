using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Data.Clean
{
    public class C3dForceplate
    {
        private static readonly string DefaultLabelTypeUnkown = "Unkown";
        private static readonly string[] DefaultLabelsType1 = new string[]{ "nFX", "nFY", "nFZ", "nPX", "nPY", "nMZ" };
        private static readonly string[] DefaultLabelsType2 = new string[]{ "nFX", "nFY", "nFZ", "nMX", "nMY", "nMZ" };
        private static readonly string[] DefaultLabelsType3 = new string[]{ "nFX12", "nFX34", "nFX14", "nFX23", "nFZ1", "nFZ2", "nFZ3", "nFZ4", };
        private static readonly string[] DefaultLabelsType4 = new string[]{ "nFX", "nFY", "nFZ", "nMX", "nMY", "nMZ" };

        private static readonly string DefaultDescriptionTypeUnkown = "No description provided";
        private static readonly string[] DefaultDescriptionType1 = new string[] { "FPn Fx force", "FPn Fy force", "FPn Fz force", "FPn X center of pressure", "FPn Y center of pressure", "FPn Z moment" };
        private static readonly string[] DefaultDescriptionType2 = new string[] { "FPn Fx force", "FPn Fy force", "FPn Fz force", "FPn Mx moment", "FPn My moment", "FPn Mz moment" };
        private static readonly string[] DefaultDescriptionType3 = new string[] { "FPn Fx force 1,2", "FPn Fx force 3,4", "FPn Fy force 1,4", "FPn Fy force 2,3", "FPn Fz force 1", "FPn Fz force 2", "FPn Fz force 3", "FPn Fz force 4" };
        private static readonly string[] DefaultDescriptionType4 = new string[] { "FPn Fx force", "FPn Fy force", "FPn Fz force", "FPn Mx moment", "FPn My moment", "FPn Mz moment" };

        // We don't apply the Calibration Matrix because I would need to check first 
        // That it is invertible. And at the moment I am not 100% sure they all can be
        // So we will have a function to return the data with calibration matrix applied
        public float[,] CalibrationMatrix = new float[,] { };
        public float[,] Corners = new float[3,4];
        public float[] Origin = new float[3];
        public ForceplateType Type = ForceplateType.UNKOWN;
        public (int, int) Zero = (0, 0);
        public C3dAnalogChannel[] Channels = new C3dAnalogChannel[] { }; // [Frame, Sample, Channel]

        public C3dForceplate() { }

        public float[,] GetAllData()
        {
            List<float[]> analogs = new List<float[]>();

            for(int idFrame = 0; idFrame < Channels[0].Data.Length; idFrame++)
            {
                List<float> frameData = new List<float>();
                for (int idChannel = 0; idChannel < Channels.Length; idChannel++)
                {
                    frameData.Add(Channels[idChannel].Data[idFrame]);
                }
                analogs.Add(frameData.ToArray());
            }

            return analogs.To2DArray();
        }

        public float[] ApplyCalMat(float[] data)
        {
            switch (Type)
            {
                case ForceplateType.TYPE_2:
                    if (CalibrationMatrix.Length > 0)
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            data[i] = data[i] * CalibrationMatrix[i, i];
                        }
                    }
                    return data;
                case ForceplateType.TYPE_4:
                    if (CalibrationMatrix.Length > 0)
                    {
                        return ArrayUtils.VecMatMultiplication(data, CalibrationMatrix);
                    }
                    else
                    {
                        return data;
                    }
                default:
                    return data;
            }
        }

        public float[,] GetAllDataWithCalMat()
        {
            List<float[]> analogs = new List<float[]>();

            for (int idFrame = 0; idFrame < Channels[0].Data.Length; idFrame++)
            {
                List<float> frameData = new List<float>();
                for (int idChannel = 0; idChannel < Channels.Length; idChannel++)
                {
                    frameData.Add(Channels[idChannel].Data[idFrame]);
                }
                analogs.Add(ApplyCalMat(frameData.ToArray()));
            }

            return analogs.To2DArray();
        }
        // TODO?
        // This one on the back burner for the time being
        //public C3dForceplate(
        //    float[,] corners,
        //    float[] origin,
        //    ForceplateType type,
        //    (int, int) zero,
        //    float[,]? calibrationMatrix = null,
        //    string[]? labels = null,
        //    string[]? descriptions = null,
        //    float[,]? data = null
        //    )
        //{
        //    CalibrationMatrix = calibrationMatrix ?? new float[,] { };
        //    Data = data ?? new float[,] { };
        //    switch (type)
        //    {
        //        case ForceplateType.TYPE_1:
        //            Label = DefaultLabelsType1;
        //            Description = DefaultDescriptionType1;
        //            break;
        //        case ForceplateType.TYPE_2:
        //            Label = DefaultLabelsType2;
        //            Description = DefaultDescriptionType2;
        //            break;
        //        case ForceplateType.TYPE_3:
        //            Label = DefaultLabelsType3;
        //            Description = DefaultDescriptionType3;
        //            break;
        //        case ForceplateType.TYPE_4:
        //            Label = DefaultLabelsType4;
        //            Description = DefaultDescriptionType4;
        //            break;
        //        default:
        //            List<string> fpLabel = new List<string> { };
        //            List<string> fpDescription = new List<string> { };
        //            if (data != null) 
        //            {
        //                for(int i=0; i<data.GetLength(2); i++) 
        //                {
        //                    fpLabel.Add(DefaultLabelTypeUnkown);
        //                    fpDescription.Add(DefaultDescriptionTypeUnkown);
        //                }
        //            }
        //            else if (calibrationMatrix != null)
        //            {
        //                for (int i = 0; i < calibrationMatrix.GetLength(0); i++)
        //                {
        //                    fpLabel.Add(DefaultLabelTypeUnkown);
        //                    fpDescription.Add(DefaultDescriptionTypeUnkown);
        //                }
        //            }
        //            Label = fpLabel.ToArray();
        //            Description = fpDescription.ToArray();
        //            break;
        //    }

        //    Type = type;
        //    Origin = origin;
        //    Type = type;
        //    Zero = zero;
        //    Corners = corners;

        //}
    }
}
