namespace SHARP3D
{
    ///<summary>
    ///This enum represent the data format flag found in the C3D header at byte 2. Without any confirmation from the C3D fundation, and that only the 0x50 value (80, "P") is viable and prelude a viable C3D file, we stick to the assumption that 0x50 means viabe C3D file and any other value means corrupted file.
    ///Alternatively some future test on the many test files distributed by the C3D organisation might shed some light on other possible viable values.
    ///</summary>
    public enum DataFormat : byte
    {
        RIGHT = 0x50,
        WRONG = 0x00,
    }
}

