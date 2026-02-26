namespace SHARP3D.Data.Data
{
    public struct C3dData
    {
        // TODO: Add resolutions of the points
        #warning Temporary fix.
        public List<C3dDataPoint[]> Points;
        //string[] PointLabels;
        //string PointsUnit;// Default: mm

        public List<float[]> Analogs;
        //string[] AnalogLabels;
        //string[] AnalogUnits;
    }
}
