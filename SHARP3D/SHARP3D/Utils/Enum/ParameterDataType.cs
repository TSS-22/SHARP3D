namespace SHARP3D.Utils.Enum
{
    /// <summary>
    /// Specifies the data types used in C3D files for parameter and data storage.
    /// </summary>
    /// <remarks>
    /// The integer values correspond to the byte size of each data type, with CHAR being a special case.
    /// </remarks>
    public enum DataType : int
    {
        /// <summary>
        /// Character data type, represented as a special case with value -1.
        /// </summary>
        CHAR = -1,

        /// <summary>
        /// 1-byte unsigned integer data type.
        /// </summary>
        BYTE = 1,

        /// <summary>
        /// 2-byte signed integer data type.
        /// </summary>
        INT16 = 2,

        /// <summary>
        /// 4-byte floating-point data type.
        /// </summary>
        FLOAT32 = 4,

        /// <summary>
        /// 2-byte unsigned integer data type.
        /// </summary>
        UINT16 = -2,
    }
}
