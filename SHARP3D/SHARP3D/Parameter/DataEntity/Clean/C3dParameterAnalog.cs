namespace SHARP3D.Parameter.DataEntity.Clean
{
    /// <summary>
    /// Analog 3D parameter data entity with value-based equality
    /// </summary>
    public class C3dParameterAnalog : IEquatable<C3dParameterAnalog>
    {
        public float GeneralScale = 1;
        public int SamplesPerFrame = 0;

        /// <summary>
        /// Compares this instance to another for value equality
        /// </summary>
        public bool Equals(C3dParameterAnother? other)
        {
            if (other is null) return false;

            // Compare with tolerance for floating-point values
            const float epsilon = 0.0001f;
            return Math.Abs(GeneralScale - other.GeneralScale) < epsilon &&
                   SamplesPerFrame == other.SamplesPerFrame;
        }

        /// <summary>
        /// Overrides base Equals method
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as C3dParameterAnalog);

        /// <summary>
        /// Overrides GetHashCode for consistency with Equals
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                // Use bitwise operations to combine hash codes reliably
                int hash = 17;
                hash = hash * 31 + SamplesPerFrame.GetHashCode();

                // For float, convert bits for consistent hashing
                hash = hash * 31 + ((GeneralScale == 0) ? 0 : BitConverter.SingleToInt32Bits(GeneralScale));

                return hash;
            }
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(C3dParameterAnalog? left, C3dParameterAnalog? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(C3dParameterAnalog? left, C3dParameterAnalog? right)
        {
            return !(left == right);
        }
    }
}