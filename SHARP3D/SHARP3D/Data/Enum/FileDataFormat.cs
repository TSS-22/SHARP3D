namespace SHARP3D
{
    ///<summary>
    ///This enum represent the data format flag found in the C3D header at byte 2. Without any confirmation from the C3D fundation, and that only the 0x50 value (80, "P") is viable and prelude a viable C3D file, we stick to the assumption that 0x50 means viabe C3D file and any other value means corrupted file.
    ///Alternatively some future test on the many test files distributed by the C3D organisation might shed some light on other possible viable values.
    ///</summary>
    public enum FileDataFormat : byte
    {
        /// <summary>
        /// Represents the RIGHT key with a value of 0x50.
        /// </summary>
        RIGHT = 0x50,
        /// <summary>
        /// Indicates an incorrect or invalid value. As of today, I have not found any indication of what should be expected for a viable C3D file other than the 0x50 value. Therefore, until further notice, any value other than 0x50 is considered as WRONG, meaning that the file is likely corrupted or not a valid C3D file.
        /// </summary>
        WRONG = 0x00,
    }
}

