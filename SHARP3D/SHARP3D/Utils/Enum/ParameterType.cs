namespace SHARP3D.Utils.Enum
{
    /// <summary>
    /// Specifies the types of parameters used in C3D files.
    /// </summary>
    /// <remarks>
    /// These types categorize parameters based on their origin and purpose within the C3D file structure.
    /// </remarks>
    public enum ParameterType : int
    {
        /// <summary>
        /// Represents an unknown or unsupported parameter type.
        /// </summary>
        UNKOWN = -1,

        /// <summary>
        /// Represents a required parameter that must be present in the C3D file.
        /// </summary>
        REQUIRED = 0,

        /// <summary>
        /// Represents an additional parameter that extends the standard C3D file format.
        /// </summary>
        ADDITIONAL = 1,

        /// <summary>
        /// Represents an application-specific parameter defined by the software application.
        /// </summary>
        APPLICATION = 2,

        /// <summary>
        /// Represents a user-defined parameter added by the end user.
        /// </summary>
        USER = 3,
    }
}