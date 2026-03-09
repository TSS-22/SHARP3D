using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter
{
    /// <summary>
    /// Represents a supported parameter in a C3D file, including its group, name, type, and descriptions.
    /// </summary>
    public record SupportedParameter
    {
        // A name and description to display for the program in case it is needed.
        /// <summary>
        /// Gets the group name of the parameter.
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public ParameterType ParameterType { get; }

        /// <summary>
        /// Gets the general description of the parameter.
        /// </summary>
        public string GeneralDescription { get; }

        // The dimensions are the index from the fortran matrix from the parameter data, as they would appear in a classic matrix.
        // For example: a matrix m*n. m-->Dimension0 and n-->Dimension1.
        // The user or us with pre-supported parameter format define which index from the fortran serialization of the c3d represent m and n.
        //int[] Dimension;
        // I don't think I need it

        // This is the meaning of the associated dimension.
        // It is here for info, but is not really necessary,
        // as a quick description similar to this should be available in the description of the parameter in the C3D file.
        /// <summary>
        /// Gets the description of each dimension of the parameter.
        /// </summary>
        /// <remarks>
        /// The dimensions are the indices from the Fortran matrix of the parameter data, as they would appear in a classic matrix.
        /// For example: a matrix m*n, where m corresponds to Dimension0 and n corresponds to Dimension1.
        /// This is the meaning of the associated dimension and is provided for informational purposes.
        /// </remarks>
        public string[] DimensionDescription { get; }


        // TODO: Finish this
        // For scalar use dimension = { 0 }
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedParameter"/> record.
        /// </summary>
        /// <param name="group">The group name of the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="parameterType">The type of the parameter.</param>
        /// <param name="generalDescription">The general description of the parameter.</param>
        /// <param name="dimensionDescription">The description of each dimension of the parameter.</param>
        public SupportedParameter(
            string group,
            string name,
            int parameterType,
            string generalDescription,
            string[] dimensionDescription
        ) { 
            Group = group;
            Name = name;
            GeneralDescription = string.IsNullOrEmpty(generalDescription)? "" : generalDescription;
            ParameterType = (ParameterType)parameterType;
            DimensionDescription = (dimensionDescription == null || dimensionDescription.Length == 0) ? new string[] { } : dimensionDescription;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SupportedParameter"/> representing an unknown parameter.
        /// </summary>
        /// <returns>A <see cref="SupportedParameter"/> instance with default values for an unknown parameter.</returns>
        public static SupportedParameter UnkownParameter() 
        {
            return new SupportedParameter(
                "UNKOWN",
                "UNKOWN",
                -1,
                "Unkown parameter. If you know this parameter, please add it in your user file following the documentation guidelines. Feel free to contact us to add it to the standard list of supported parameter.",
                null
                );
        }
        //public SupportedParameter()
        //{
        //    Group = "UNKOWN";
        //    Name = "UNKOWN";
        //    GeneralDescription = "Unkown parameter. If you know this parameter, please add it in your user file following the documentation guidelines. Feel free to contact us to add it to the standard list of supported parameter.";
        //    ParameterType = ParameterType.UNKOWN;
        //    DimensionDescription = new string[] { };
        //}
            
       
    }
}
