namespace SHARP3D.Data.Data
{
    public struct C3dDataPoint : IEquatable<C3dDataPoint>
    {
        public float[] Data;

        public float AverageResidual;

        public bool[] CameraMask;

        public bool Raw;
        public bool Valid;

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

        public override bool Equals(object obj)
        {
            return obj is C3dDataPoint other && Equals(other);
        }

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

        public static bool operator ==(C3dDataPoint left, C3dDataPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(C3dDataPoint left, C3dDataPoint right)
        {
            return !left.Equals(right);
        }

    }

}
