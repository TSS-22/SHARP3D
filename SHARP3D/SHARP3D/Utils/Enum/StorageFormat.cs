namespace SHARP3D.Utils.Enum
{
    ///<summary>
    ///This enum represent the data storage format flag found in the C3D header at byte 2. 
    ///</summary>
    public enum StorageFormat : byte
    {
        /// <summary>
        /// Represents the original storage flag value of 0x50.
        /// </summary>
        ORIGINAL = 0x50,

        /// <summary>
        /// Indicates an unkown storage flag. If you are reading a valid C3D file, please send us an email so we can add this flag in the list of recognized flag.
        /// </summary>
        UNKOWN = 0x00,
    }
}

