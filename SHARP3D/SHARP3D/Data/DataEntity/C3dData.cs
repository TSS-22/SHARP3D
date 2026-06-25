namespace SHARP3D.Data.DataEntity
{
    public class C3dData
    {
        // Should I seperate it into Point and Analog?
        public float?[,,]? Point = null; // Non valid -> null values | No point data -> null
        public float?[,]? Residual = null; // Non raw -> null values  | No point data -> null
        public bool[,,]? CameraMask = null;// No point data -> null

        public float[,]? Analog = null; // | No analog data -> null
        public float[][,]? ForcePlate = null;// | No force plate data -> null
    }

}
