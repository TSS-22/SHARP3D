namespace SHARP3D.Parameter.Data
{
    /// <summary>
    /// Represents a group of parameters in a C3D file, including metadata and a list of parameters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Byte: 1
    /// Reserved for parameter file use.
    /// </para>
    /// <para>
    /// Byte: 2
    /// Reserved for parameter file use.
    /// </para>
    /// <para>
    /// Byte: 3
    /// Parameter section size in 512 byte blocks.
    /// </para>
    /// <para>
    /// Byte: 4
    /// Processor type.
    /// <para>
    /// Value = 0x53 + processor type
    /// </para>
    /// <para>
    /// Value (in decimal) = 83 + processor type
    /// </para>
    /// <list type="table">
    ///     <item>
    ///         <term>0x54</term>
    ///         <description>Processor type 1: INTEL</description>
    ///     </item>
    ///     <item>
    ///         <term>0x55</term>
    ///         <description>Processor type 2: DEC (VAX,PDP-11)</description>
    ///     </item>
    ///     <item>
    ///         <term>0x56</term>
    ///         <description>Processor type 3: MIPS (SGI/MIPS)</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </remarks>
    public struct C3dParameterGroup : IEquatable<C3dParameterGroup>
    {
        /// <summary>
        /// <para>
        /// Byte position: 1. Length: 1
        /// </para>
        /// The length of the group's name.
        /// </summary>
        public sbyte NameLength;

        /// <summary>
        /// <para>
        /// Byte position: 2. Length: 1
        /// </para>
        /// The unique identifier of the group.
        /// </summary>
        public int Id;

        /// <summary>
        /// <para>
        /// Byte position: 3. Length: n
        /// </para>
        /// The name of the group.
        /// </summary>
        public string Name;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n. Length: 2
        /// </para>
        /// A pointer to the next parameter group structure in the C3D file.
        /// </summary>
        public int PointerNextParameterStruct; // From the pointer position to the next data structure

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 2. Length: 1
        /// </para>
        /// The length of the group's description.
        /// </summary>
        public int DescriptionLength;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 3. Length: m
        /// </para>
        /// The actual length of the group's description, accounting for UTF-8 encoding.
        /// </summary>
        /// <remarks>
        /// TODO: Check if it is necessary to make the distinction between <see cref="DescriptionLength"/> and <see cref="ActualDescriptionLength"/>.
        /// </remarks>
        public long ActualDescriptionLength; // Because of UTF-8. TODO: Check if it is necessary to make the distinction.

        /// <summary>
        /// The description of the group.
        /// </summary>
        public string Description;

        /// <summary>
        /// Indicates whether the group is locked.
        /// </summary>
        public bool Locked; // For later and the correctors

        /// <summary>
        /// The list of parameters contained in this group.
        /// </summary>
        public List<C3dParameter> Parameters;

        /// <summary>
        /// Determines whether the current <see cref="C3dParameterGroup"/> instance is equal to another <see cref="C3dParameterGroup"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="C3dParameterGroup"/> instance to compare with the current instance.</param>
        /// <returns><c>true</c> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Determines whether the current <see cref="C3dParameterGroup"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the current instance is equal to the <paramref name="obj"/> parameter; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is C3dParameterGroup other && Equals(other);
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dParameterGroup"/> instances are equal.
        /// </summary>
        /// <param name="group1">The first <see cref="C3dParameterGroup"/> instance to compare.</param>
        /// <param name="group2">The second <see cref="C3dParameterGroup"/> instance to compare.</param>
        /// <returns><c>true</c> if <paramref name="group1"/> and <paramref name="group2"/> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(C3dParameterGroup group1, C3dParameterGroup group2)
        {
            return group1.Equals(group2);
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dParameterGroup"/> instances are not equal.
        /// </summary>
        /// <param name="group1">The first <see cref="C3dParameterGroup"/> instance to compare.</param>
        /// <param name="group2">The second <see cref="C3dParameterGroup"/> instance to compare.</param>
        /// <returns><c>true</c> if <paramref name="group1"/> and <paramref name="group2"/> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(C3dParameterGroup group1, C3dParameterGroup group2)
        {
            return !group1.Equals(group2);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="C3dParameterGroup"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
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
