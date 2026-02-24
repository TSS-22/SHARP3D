using SHARP3D.Utils.Enum;
using System.Text.Json.Serialization;

namespace SHARP3D.Parameter
{
    public struct SupportedParameter
    {
        // A name and description to display for the program in case it is needed.
        public string Group { get; }
        public string Name { get; }
        public ParameterType ParameterType { get; }
        public string GeneralDescription { get; }

        // The dimensions are the index from the fortran matrix from the parameter data, as they would appear in a classic matrix.
        // For example: a matrix m*n. m-->Dimension0 and n-->Dimension1.
        // The user or us with pre-supported parameter format define which index from the fortran serialization of the c3d represent m and n.
        //int[] Dimension;
        // I don't think I need it

        // This is the meaning of the associated dimension.
        // It is here for info, but is not really necessary,
        // as a quick description similar to this should be available in the description of the parameter in the C3D file.
        public string[] DimensionDescription { get; }


        // TODO: Finish this
        // For scalar use dimension = { 0 }
        public SupportedParameter(
            string group,
            string name,
            ParameterType parameterType,
            string generalDescription,
            string[] dimensionDescription
        ) { 
            Group = group;
            Name = name;
            GeneralDescription = string.IsNullOrEmpty(generalDescription)? "" : generalDescription;
            ParameterType = parameterType;
            DimensionDescription = (dimensionDescription == null || dimensionDescription.Length == 0) ? new string[] { } : dimensionDescription;
        }
    }
}
