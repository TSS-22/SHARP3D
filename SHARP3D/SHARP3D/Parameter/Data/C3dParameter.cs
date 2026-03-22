using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.Data
{
    /// <summary>
    /// Represents a parameter in a C3D file, including its metadata, data type, dimensions, and data.
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

    public struct C3dParameter : IEquatable<C3dParameter>
    {
        /// <summary>
        /// <para>
        /// Byte position: 1. Length: 1
        /// </para>
        /// The length of the parameter's name.
        /// </summary>
        public sbyte NameLength;

        /// <summary>
        /// <para>
        /// Byte position: 2. Length: 1
        /// </para>
        /// The unique identifier of the parameter.
        /// </summary>
        public int Id;

        /// <summary>
        /// <para>
        /// Byte position: 3. Length: n
        /// </para>
        /// The name of the parameter, in ASCII character).
        /// </summary>
        public string Name;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n. Length: 2
        /// </para>
        /// An unsigned integer pointer to the next parameter structure in the C3D file.
        /// </summary>
        public int PointerNextParameterStruct;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 2. Length: 1
        /// </para>
        /// The data type of the parameter.
        /// </summary>
        public DataType DataTypeFile;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 3. Length: 1
        /// </para>
        /// The number of dimensions of the parameter's data.
        /// </summary>
        public int NbOfDimensions;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 4. Length: d
        /// </para>
        /// The dimensions of the parameter's data. This can be null if the parameter has no dimensions.
        /// </summary>
        public int[]? Dimensions;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 4 + d. Length: t
        /// </para>
        /// The data associated with the parameter.
        /// </summary>
        public Array Data;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 4 + d + t. Length: 1
        /// </para>
        /// The length of the parameter's description.
        /// </summary>
        public int DescriptionLength;

        /// <summary>
        /// <para>
        /// Byte position: 3 + n + 4 + d + t + 1. Length: m
        /// </para>
        /// The description of the parameter.
        /// </summary>
        public string Description;

        /// <summary>
        /// Indicates whether the parameter is locked.
        /// </summary>
        public bool Locked;

        /// <summary>
        /// Indicates whether the parameter is supported.
        /// </summary>
        public SupportedParameter Supported;

        /// <summary>
        /// Determines whether the current <see cref="C3dParameter"/> instance is equal to another <see cref="C3dParameter"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="C3dParameter"/> instance to compare with the current instance.</param>
        /// <returns><c>true</c> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
        public bool Equals(C3dParameter other)
        {
            // Compare all value-type fields
            if (NameLength != other.NameLength ||
                Id != other.Id ||
                PointerNextParameterStruct != other.PointerNextParameterStruct ||
                DataTypeFile != other.DataTypeFile ||
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
                     Data.Length != other.Data.Length
                    )
            {
                return false;
            }
            else if (Data.Length == other.Data.Length)
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    if (!object.Equals(Data.GetValue(i), other.Data.GetValue(i)))
                    {
                        return false;
                    }
                }
                
            }
                return true;
        }

        /// <summary>
        /// Determines whether the current <see cref="C3dParameter"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the current instance is equal to the <paramref name="obj"/> parameter; otherwise, <c>false</c>.</returns>
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
                hash = hash * 23 + DataTypeFile.GetHashCode();
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

        /// <summary>
        /// Determines whether two specified <see cref="C3dParameter"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dParameter"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dParameter"/> instance to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(C3dParameter left, C3dParameter right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dParameter"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dParameter"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dParameter"/> instance to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(C3dParameter left, C3dParameter right)
        {
            return !left.Equals(right);
        }

    }
}

