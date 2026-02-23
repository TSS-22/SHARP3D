namespace SHARP3D.Parameter
{
    public class SupportedParameter
    {
        // A name and description to display for the program in case it is needed.
        string Group;
        string Name;
        string GeneralDescription;

        // The dimensions are the index from the fortran matrix from the parameter data, as they would appear in a classic matrix.
        // For example: a matrix m*n. m-->Dimension0 and n-->Dimension1.
        // The user or us with pre-supported parameter format define which index from the fortran serialization of the c3d represent m and n.
        //int[] Dimension;
        // I don't think I need it

        // This is the meaning of the associated dimension.
        // It is here for info, but is not really necessary,
        // as a quick description similar to this should be available in the description of the parameter in the C3D file.
        string[] DimensionDescription;


        // TODO: Finish this
        // For scalar use dimension = { 0 }
        protected SupportedParameter(
            string group,
            string name,
            string generalDescription = "",
            int[]? dimension = null,
            string[]? dimensionDescription = null
        ) {
            Group = group;
            Name = name;
            GeneralDescription = generalDescription;
            //Dimension = dimension == null? new int[] { 0 } : (int[])dimension;
            DimensionDescription = dimensionDescription == null ? new string[] { "" } : dimensionDescription;
        }
    }
}
