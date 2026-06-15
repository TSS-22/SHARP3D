using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.DataEntity
{
    public class C3dParameterForcePlate
    {
        public int[] Channel = new int[] { };
        public float[] Corners = new float[] { };
        public float[] Origin = new float[] { };
        public ForcePlateType Type = ForcePlateType.UNKOWN;
        public int Used = 0;
        public (int, int) Zero = (0, 0);
    }
}
