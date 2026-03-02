namespace SHARP3D.Data.Data
{
    public struct C3dData : IEquatable<C3dData>
    {
        // TODO: Add resolutions of the points
        #warning Temporary fix.
        public List<C3dDataPoint[]> Points;
        //string[] PointLabels;
        //string PointsUnit;// Default: mm

        public List<float[][]> Analogs;
        //string[] AnalogLabels;
        //string[] AnalogUnits;

        public bool Equals(C3dData other)
        {
            // Compare Points: List of C3dDataPoint[]
            if (Points == null && other.Points == null)
            {
                // Both are null, continue
            }
            else if (Points == null || other.Points == null || Points.Count != other.Points.Count)
                return false;
            else
            {
                for (int i = 0; i < Points.Count; i++)
                {
                    C3dDataPoint[] thisPoints = Points[i];
                    C3dDataPoint[] otherPoints = other.Points[i];
                    if (thisPoints == null && otherPoints == null)
                        continue;
                    if (thisPoints == null || otherPoints == null || thisPoints.Length != otherPoints.Length)
                        return false;
                    for (int j = 0; j < thisPoints.Length; j++)
                    {
                        if (!thisPoints[j].Equals(otherPoints[j]))
                            return false;
                    }
                }
            }

            // Compare Analogs: List of float[][]
            if (Analogs == null && other.Analogs == null)
            {
                // Both are null, continue
            }
            else if (Analogs == null || other.Analogs == null || Analogs.Count != other.Analogs.Count)
                return false;
            else
            {
                for (int i = 0; i < Analogs.Count; i++)
                {
                    float[][] thisAnalogs = Analogs[i];
                    float[][] otherAnalogs = other.Analogs[i];
                    if (thisAnalogs == null && otherAnalogs == null)
                        continue;
                    if (thisAnalogs == null || otherAnalogs == null || thisAnalogs.Length != otherAnalogs.Length)
                        return false;
                    for (int j = 0; j < thisAnalogs.Length; j++)
                    {
                        float[] thisArray = thisAnalogs[j];
                        float[] otherArray = otherAnalogs[j];
                        if (thisArray == null && otherArray == null)
                            continue;
                        if (thisArray == null || otherArray == null || thisArray.Length != otherArray.Length)
                            return false;
                        for (int k = 0; k < thisArray.Length; k++)
                        {
                            if (thisArray[k] != otherArray[k])
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is C3dData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                // Include Points in hash code
                if (Points != null)
                {
                    foreach (var pointArray in Points)
                    {
                        if (pointArray != null)
                        {
                            foreach (var point in pointArray)
                                hash = hash * 23 + (point?.GetHashCode() ?? 0);
                        }
                    }
                }

                // Include Analogs in hash code
                if (Analogs != null)
                {
                    foreach (var analogArray in Analogs)
                    {
                        if (analogArray != null)
                        {
                            foreach (var innerArray in analogArray)
                            {
                                if (innerArray != null)
                                {
                                    foreach (var value in innerArray)
                                        hash = hash * 23 + value.GetHashCode();
                                }
                            }
                        }
                    }
                }

                return hash;
            }
        }

        public static bool operator ==(C3dData left, C3dData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(C3dData left, C3dData right)
        {
            return !left.Equals(right);
        }
    }
}
