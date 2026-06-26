using SHARP3D.Utils.Enum;

namespace SHARP3D
{
    public class C3dForceplate
    {
        private static readonly string[] DefaultLabelsTypeUnkown = new string[]{};
        private static readonly string[] DefaultLabelsType1 = new string[]{ "nFX", "nFY", "nFZ", "nPX", "nPY", "nMZ" };
        private static readonly string[] DefaultLabelsType2 = new string[]{ "nFX", "nFY", "nFZ", "nMX", "nMY", "nMZ" };
        private static readonly string[] DefaultLabelsType3 = new string[]{ "nFX12", "nFX34", "nFX14", "nFX23", "nFZ1", "nFZ2", "nFZ3", "nFZ4", };
        private static readonly string[] DefaultLabelsType4 = new string[]{ "nFX", "nFY", "nFZ", "nMX", "nMY", "nMZ" };

        private static readonly string[] DefaultDescriptionTypeUnkown = new string[] { };
        private static readonly string[] DefaultDescriptionType1 = new string[] { "FPn Fx force", "FPn Fy force", "FPn Fz force", "FPn X center of pressure", "FPn Y center of pressure", "FPn Z moment" };
        private static readonly string[] DefaultDescriptionType2 = new string[] { "FPn Fx force", "FPn Fy force", "FPn Fz force", "FPn Mx moment", "FPn My moment", "FPn Mz moment" };
        private static readonly string[] DefaultDescriptionType3 = new string[] { "FPn Fx force 1,2", "FPn Fx force 3,4", "FPn Fy force 1,4", "FPn Fy force 2,3", "FPn Fz force 1", "FPn Fz force 2", "FPn Fz force 3", "FPn Fz force 4" };
        private static readonly string[] DefaultDescriptionType4 = new string[] { "FPn Fx force", "FPn Fy force", "FPn Fz force", "FPn Mx moment", "FPn My moment", "FPn Mz moment" };

        public float[,] CalibrationMatrix = new float[,] { };
        public float[,] Corners = new float[3,4];
        public string[] Labels = new string[] { };
        public string[] Descriptions = new string[] { };
        public float[] Origin = new float[3];
        public ForceplateType Type = ForceplateType.UNKOWN;
        public (int, int) Zero = (0, 0);
        public float[,,] Data = new float[,,] { };

        public C3dForceplate() { }

        public C3dForceplate(
            float[,] corners,
            float[] origin,
            ForceplateType type,
            (int, int) zero,
            float[,]? calibrationMatrix = null,
            string[]? labels = null,
            string[]? descriptions = null,
            float[,,]? data = null
            )
        {
            CalibrationMatrix = calibrationMatrix ?? new float[,] { };
            Data = data ?? new float[,,] { };
            switch (type)
            {
                case ForceplateType.TYPE_1:
                    Labels = DefaultLabelsType1;
                    Descriptions = DefaultDescriptionType1;
                    break;
                case ForceplateType.TYPE_2:
                    Labels = DefaultLabelsType2;
                    Descriptions = DefaultDescriptionType2;
                    break;
                case ForceplateType.TYPE_3:
                    Labels = DefaultLabelsType3;
                    Descriptions = DefaultDescriptionType3;
                    break;
                case ForceplateType.TYPE_4:
                    Labels = DefaultLabelsType4;
                    Descriptions = DefaultDescriptionType4;
                    break;
                default:
                    Labels = DefaultLabelsTypeUnkown;
                    Descriptions = DefaultDescriptionTypeUnkown;
                    break;
            }

            Type = type;
            Origin = origin;
            Type = type;
            Zero = zero;
            Corners = corners;

        }
    }
}
