namespace SHARP3D.Data.Clean
{
    public class C3dPointTrajectory
    {
        // TODO: label should be unique.
        public string Label = "Unkown";
        public string Description = "No description provided for trajectory.";

        public float?[,] Point = new float?[,] { }; // Non valid -> null values | No point data -> empty
        public float?[] Residual = new float?[] { }; // Non raw -> null values  | No point data -> empty
        public bool[,] CameraMask = new bool[,] { };// No point data -> empty
    }
}
