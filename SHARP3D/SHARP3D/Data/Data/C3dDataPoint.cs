namespace SHARP3D.Data.Data
{
    /// <summary>
    /// Represents a single 3D data point in a C3D file, including its coordinates, residual, camera visibility, and validity.
    /// </summary>
    public struct C3dDataPoint : IEquatable<C3dDataPoint>
    {
        /// <summary>
        /// Gets or sets the coordinate data of the 3D point.
        /// </summary>
        /// <remarks>
        /// This array contains the X, Y, and Z coordinates of the point.
        /// </remarks>
        public float[] Data;

        /// <summary>
        /// Gets or sets the average residual of the 3D point.
        /// </summary>
        /// <remarks>
        /// The residual represents the average distance between the reconstructed 3D point and its 2D projections on the camera images.
        /// </remarks>
        public float AverageResidual;

        /// <summary>
        /// Gets or sets a mask indicating which cameras detected this 3D point.
        /// </summary>
        /// <remarks>
        /// Each element in the array corresponds to a camera. A value of <c>true</c> indicates that the point was detected by that camera.
        /// </remarks>
        public bool[] CameraMask;

        /// <summary>
        /// Gets or sets a value indicating whether this point is raw (unprocessed) data.
        /// </summary>
        public bool Raw;

        /// <summary>
        /// Gets or sets a value indicating whether this point is valid.
        /// </summary>
        public bool Valid;


        /// <summary>
        /// Determines whether the current <see cref="C3dDataPoint"/> instance is equal to another <see cref="C3dDataPoint"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="C3dDataPoint"/> instance to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(C3dDataPoint other)
        {
            // Compare AverageResidual, Raw, and Valid
            if (AverageResidual != other.AverageResidual ||
                Raw != other.Raw ||
                Valid != other.Valid)
                return false;

            // Compare Data: float[]
            if (Data == null && other.Data == null)
            {
                // Both are null, continue
            }
            else if (Data == null || other.Data == null || Data.Length != other.Data.Length)
                return false;
            else
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    if (Data[i] != other.Data[i])
                        return false;
                }
            }

            // Compare CameraMask: bool[]
            if (CameraMask == null && other.CameraMask == null)
            {
                // Both are null, continue
            }
            else if (CameraMask == null || other.CameraMask == null || CameraMask.Length != other.CameraMask.Length)
                return false;
            else
            {
                for (int i = 0; i < CameraMask.Length; i++)
                {
                    if (CameraMask[i] != other.CameraMask[i])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the current <see cref="C3dDataPoint"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the current instance is equal to the <paramref name="obj"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is C3dDataPoint other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <remarks>
        /// The hash code is calculated using all fields of the <see cref="C3dDataPoint"/> struct.
        /// </remarks>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + AverageResidual.GetHashCode();
                hash = hash * 23 + Raw.GetHashCode();
                hash = hash * 23 + Valid.GetHashCode();

                // Include Data in hash code
                if (Data != null)
                {
                    foreach (var value in Data)
                        hash = hash * 23 + value.GetHashCode();
                }

                // Include CameraMask in hash code
                if (CameraMask != null)
                {
                    foreach (var value in CameraMask)
                        hash = hash * 23 + value.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dDataPoint"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dDataPoint"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dDataPoint"/> instance to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(C3dDataPoint left, C3dDataPoint right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dDataPoint"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dDataPoint"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dDataPoint"/> instance to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(C3dDataPoint left, C3dDataPoint right)
        {
            return !left.Equals(right);
        }

    }

}
