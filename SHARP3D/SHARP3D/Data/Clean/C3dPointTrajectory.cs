namespace SHARP3D.Data.Clean
{
    internal class C3dPointTrajectory
    {
        public string Label = "Unkown";
        public string description = "No description provided";

        public float?[,] Point = new float?[,] { }; // Non valid -> null values | No point data -> empty
        public float?[] Residual = new float?[] { }; // Non raw -> null values  | No point data -> empty
        public bool[,] CameraMask = new bool[,] { };// No point data -> empty
    }
}
