namespace SHARP3D.Data.DataEntity
{
    public class C3dData
    {
        // Should I seperate it into Point and Analog?
        public float?[,,]? Point; // Non valid -> null values | No point data -> null
        public float?[]? Residual; // Non raw -> null values  | No point data -> null
        public bool[,,,,,,]? CameraMask;// No point data -> null

        public float[]? Analog; // | No analog data -> null
        public float[,,]? ForcePlate;// | No force plate data -> null
    }
}
