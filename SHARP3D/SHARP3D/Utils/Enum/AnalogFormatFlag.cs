namespace SHARP3D.Utils.Enum
{
    /// <summary>
    /// Specifies the <see href="https://tss-22.github.io/SHARP3D/c3d_docs/parameters/required/analog-format.html">ANALOG:FORMAT Parameter</see> value.
    /// </summary>
    public enum AnalogFormatFlag : int
    {
        /// <summary>
        /// Default <see href="https://tss-22.github.io/SHARP3D/c3d_docs/parameters/required/analog-format.html">ANALOG:FORMAT</see>.
        /// </summary>
        SIGNED = 0,

        /// <summary>
        /// If <see href="https://tss-22.github.io/SHARP3D/c3d_docs/data/analog.html">Analog Data</see> 
        /// and associated <see href="https://tss-22.github.io/SHARP3D/c3d_docs/parameters/c3d-parameter-section.html">Parameters</see> are stored as unisgned 16-bit integers.
        /// </summary>
        UNSIGNED = 1,

        /// <summary>
        /// If <see href="https://tss-22.github.io/SHARP3D/c3d_docs/parameters/required/analog-format.html">ANALOG:FORMAT</see> wasn't set.
        /// </summary>
        UNKOWN = -1
    }
}