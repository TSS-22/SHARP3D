namespace SHARP3D.Data.Clean
{
    public class C3dData
    {
        //// Should I seperate it into Point and Analog?
        //public float?[,,]? Point = null; // Non valid -> null values | No point data -> null
        //public float?[,]? Residual = null; // Non raw -> null values  | No point data -> null
        //public bool[,,]? CameraMask = null;// No point data -> null

        //public float[,]? Analog = null; // | No analog data -> null
        //public float[][,]? ForcePlate = null;// | No force plate data -> null
        C3dPointTrajectory[] Points = new C3dPointTrajectory[] { };
        C3dAnalogChannel[] Analogs = new C3dAnalogChannel[] { };
        C3dForceplate[] Forceplates = new C3dForceplate[] { };
    }

}
