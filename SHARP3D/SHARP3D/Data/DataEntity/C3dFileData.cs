namespace SHARP3D.Data.DataEntity
{
    /// <summary>
    /// Represents 3D data in a C3D file, including points and analog data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This struct stores 3D point data and analog data as lists of arrays.
    /// </para>
    /// <para>
    /// <strong>Points:</strong> A list of <see cref="C3dFileDataPoint"/> arrays, where each array represents a frame of 3D points.
    /// </para>
    /// <para>
    /// <strong>Analogs:</strong> A list of 2D float arrays, where each array represents a frame of analog data.
    /// </para>
    /// <para>
    /// <strong>Equality:</strong> Two <see cref="C3dFileData"/> instances are considered equal if their <see cref="Points"/> and <see cref="Analogs"/> fields are equal.
    /// </para>
    /// </remarks>
    public struct C3dFileData : IEquatable<C3dFileData>
    {
        /// <summary>
        /// Gets or sets a list of 3D point arrays, where each array represents a frame of 3D points.
        /// </summary>
        /// <remarks>
        /// TODO: Add resolutions of the points.
        /// </remarks>
        #warning Temporary fix.
        // TODO: Add resolutions of the points
        public List<C3dFileDataPoint[]> Points;
        //string[] PointLabels;
        //string PointsUnit;// Default: mm

        /// <summary>
        /// Gets or sets a list of 2D float arrays, where each array represents a frame of analog data.
        /// </summary>
        public List<float[][]> Analogs;
        //string[] AnalogLabels;
        //string[] AnalogUnits;

        /// <summary>
        /// Determines whether the current <see cref="C3dFileData"/> instance is equal to another <see cref="C3dFileData"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="C3dFileData"/> instance to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(C3dFileData other)
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
                    C3dFileDataPoint[] thisPoints = Points[i];
                    C3dFileDataPoint[] otherPoints = other.Points[i];
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

        /// <summary>
        /// Determines whether the current <see cref="C3dFileData"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the current instance is equal to the <paramref name="obj"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is C3dFileData other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
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
                                hash = hash * 23 + point.GetHashCode();
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

        /// <summary>
        /// Determines whether two specified <see cref="C3dFileData"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dFileData"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dFileData"/> instance to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(C3dFileData left, C3dFileData right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dFileData"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dFileData"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dFileData"/> instance to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(C3dFileData left, C3dFileData right)
        {
            return !left.Equals(right);
        }
    }
}
