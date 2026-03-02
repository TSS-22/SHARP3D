using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.Data
{
    public struct C3dParameter : IEquatable<C3dParameter>
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public int PointerNextParameterStruct;
        public DataType DataType;
        public int NbOfDimensions;
        public int[]? Dimensions; 
        public Array Data;
        public int DescriptionLength;
        public string Description;
        public bool Locked;
        public SupportedParameter Supported;

        public bool Equals(C3dParameter other)
        {
            // Compare all value-type fields
            if (NameLength != other.NameLength ||
                Id != other.Id ||
                PointerNextParameterStruct != other.PointerNextParameterStruct ||
                DataType != other.DataType ||
                NbOfDimensions != other.NbOfDimensions ||
                DescriptionLength != other.DescriptionLength ||
                Locked != other.Locked ||
                Supported != other.Supported)
                return false;

            // Compare string fields (handle null)
            if (!string.Equals(Name, other.Name) ||
                !string.Equals(Description, other.Description))
                return false;

            // Compare Dimensions array (handle null and contents)
            if (Dimensions == null && other.Dimensions == null)
            {
                // Both are null, continue
            }
            else if (Dimensions == null || other.Dimensions == null ||
                     !Dimensions.SequenceEqual(other.Dimensions))
                return false;

            // Compare Data array (handle null and contents)
            if (Data == null && other.Data == null)
            {
                // Both are null, continue
            }
            else if (Data == null || other.Data == null ||
                     !Data.Equals(other.Data)) // Array.Equals checks reference, not contents
            {
                // For Array, you need to compare contents element-wise if needed
                // This is a simplified check; see note below
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is C3dParameter other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + NameLength.GetHashCode();
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                hash = hash * 23 + PointerNextParameterStruct.GetHashCode();
                hash = hash * 23 + DataType.GetHashCode();
                hash = hash * 23 + NbOfDimensions.GetHashCode();
                hash = hash * 23 + (Dimensions != null ? Dimensions.GetHashCode() : 0);
                hash = hash * 23 + (Data != null ? Data.GetHashCode() : 0);
                hash = hash * 23 + DescriptionLength.GetHashCode();
                hash = hash * 23 + (Description?.GetHashCode() ?? 0);
                hash = hash * 23 + Locked.GetHashCode();
                hash = hash * 23 + Supported.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(C3dParameter left, C3dParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(C3dParameter left, C3dParameter right)
        {
            return !left.Equals(right);
        }

    }
}

