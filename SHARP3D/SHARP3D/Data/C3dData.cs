namespace SHARP3D.Data
{
    internal struct C3dData
    {
        C3dDataPoint[,] Points;
        string[] PointLabels;
        string PointsUnit;// Default: mm

        float[,] Analogs;
        string[] AnalogLabels;
        string[] AnalogUnits;


    }
}
