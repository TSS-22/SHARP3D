namespace SHARP3D.Utils
{
    /// <summary>
    /// Provides program-wide constants for SHARP3D, including arrays representing signed and unsigned string literals.
    /// </summary>
    internal static class Sharp3dConstants
    {
        /// <summary>
        /// A read-only <see cref="Array"/> of <see cref="char"/> representing the string "SIGNED".
        /// </summary>
        public static readonly Array SignedArrayString;

        /// <summary>
        /// A read-only <see cref="Array"/> of <see cref="char"/> representing the string "UNSIGNED".
        /// </summary>
        public static readonly Array UnsignedArrayString;

        /// <summary>
        /// Initializes the <see cref="SignedArrayString"/> and <see cref="UnsignedArrayString"/> arrays.
        /// </summary>
        static Sharp3dConstants()
        {
            // Initialize SignedArrayString
            SignedArrayString = Array.CreateInstance(typeof(char), "SIGNED".Length);
            for (int i = 0; i < "SIGNED".Length; i++)
            {
                SignedArrayString.SetValue("SIGNED"[i], i);
            }

            // Initialize UnsignedArrayString
            UnsignedArrayString = Array.CreateInstance(typeof(char), "UNSIGNED".Length);
            for (int i = 0; i < "UNSIGNED".Length; i++)
            {
                UnsignedArrayString.SetValue("UNSIGNED"[i], i);
            }
        }
    }
}