namespace SHARP3D.Parameter.Data
{
    public struct C3dParameterGroup : IEquatable<C3dParameterGroup>
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public int PointerNextParameterStruct; // From the pointer position to the next data structure
        public int DescriptionLength;
        public long ActualDescriptionLength; // Because of UTF-8. TODO: Check if it is necessary to make the distinction.
        public string Description;
        public bool Locked; // For later and the correctors
        public List<C3dParameter> Parameters;

        public bool Equals(C3dParameterGroup other) 
        {
            return NameLength == other.NameLength &&
                Id == other.Id &&
                Name == other.Name &&
                PointerNextParameterStruct == other.PointerNextParameterStruct &&
                DescriptionLength == other.DescriptionLength &&
                ActualDescriptionLength == other.ActualDescriptionLength &&
                string.Equals(Description, other.Description) &&
                Locked == other.Locked &&
                (Parameters == null && other.Parameters == null ||
                Parameters != null && other.Parameters != null &&
                Parameters.SequenceEqual(other.Parameters));
        }

        public override bool Equals(object obj)
        {
            return obj is C3dParameterGroup other && Equals(other);
        }

        public static bool operator ==(C3dParameterGroup group1, C3dParameterGroup group2)
        {
            return group1.Equals(group2);
        }

        public static bool operator !=(C3dParameterGroup group1, C3dParameterGroup group2)
        {
            return !group1.Equals(group2);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine
            {
                int hash = 17;
                hash = hash * 23 + NameLength.GetHashCode();
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                hash = hash * 23 + PointerNextParameterStruct.GetHashCode();
                hash = hash * 23 + DescriptionLength.GetHashCode();
                hash = hash * 23 + ActualDescriptionLength.GetHashCode();
                hash = hash * 23 + (Description?.GetHashCode() ?? 0);
                hash = hash * 23 + Locked.GetHashCode();

                if (Parameters != null)
                {
                    foreach (var parameter in Parameters)
                    {
                        hash = hash * 23 + (parameter.GetHashCode());
                    }
                }

                return hash;
            }
        }
    }
}
