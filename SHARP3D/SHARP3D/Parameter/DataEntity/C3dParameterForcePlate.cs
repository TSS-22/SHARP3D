using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.DataEntity
{
    public class C3dParameterForceplate
    {
        public int[] Channels = new int[] { };
        public float[] Corners = new float[] { };
        public string[] Labels;
        public float[] Origin = new float[] { };
        public ForcePlateType Type = ForcePlateType.UNKOWN;
        public int Used = 0;
        public (int, int) Zero = (0, 0);
    }
}
