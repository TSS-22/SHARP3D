namespace SHARP3D.Parameter.DataEntity.Clean
{
    /// <summary>
    /// Point 3D parameter data entity with value-based equality
    /// </summary>
    public class C3dParameterPoint : IEquatable<C3dParameterPoint>
    {
        public float Rate = 0;
        // Scale is calculated when saving to C3D file, not stored here
        public string Units = "mm";
        
        /// <summary>
        /// Compares this instance to another for value equality
        /// </summary>
        public bool Equals(C3dParameterPoint? other)
        {
            if (other is null) return false;

            // Compare with tolerance for floating-point values
            const float epsilon = 0.0001f;
            return Math.Abs(Rate - other.Rate) < epsilon &&
                   (Units ?? "") == (other.Units ?? "");
        }

        /// <summary>
        /// Overrides base Equals method
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as C3dParameterPoint);

        /// <summary>
        /// Overrides GetHashCode for consistency with Equals
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                // Use bitwise operations to combine hash codes reliably
                int hash = 17;

                // For float, convert bits for consistent hashing
                hash = hash * 31 + ((Rate == 0) ? 0 : BitConverter.SingleToInt32Bits(Rate));

                // String hash code (handles null)
                hash = hash * 31 + (Units?.GetHashCode() ?? 0);

                return hash;
            }
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(C3dParameterPoint? left, C3dParameterPoint? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(C3dParameterPoint? left, C3dParameterPoint? right)
        {
            return !(left == right);
        }
    }
}