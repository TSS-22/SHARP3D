using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.DataEntity
{
    public class C3dParameterForceplate
    {
        public int[][] Channel = new int[][] { }; // We keep the typo to be consistant with the C3D Doc. It won't be used by the user anyway.
        public float[,,] Corners = new float[,,] { };
        public string[][] Labels = new string[][] { };
        public string[][] Descriptions = new string[][] { };
        public float[,] Origin = new float[,] { };
        public ForceplateType[] Type = new ForceplateType[] { };
        public int Used = 0;
        public (int, int) Zero = (0, 0);
    }
}
