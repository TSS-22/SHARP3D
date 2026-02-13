namespace SHARP3D.Parameter.Supported
{
    internal abstract class SupportedParameter
    {
        // A name and description to display for theprogramnin case it is needed.
        string Name;
        string Description;

        // The dimensions are the index from the fortran matrix from the parameter data, as they would appear in a classic matrix.
        // For example: a matrix m*n. m-->Dimension0 and n-->Dimension1.
        // The user or us with pre-supported parameter format define which index from the fortran serialization of the c3d represent m and n.
        int Dimension0;
        int Dimension1;
        int Dimension2;
        int Dimension3;
        int Dimension4;
        int Dimension5;
        int Dimension6;
        int Dimension7;

        // This is the meaning of the associated dimension.
        // It is here for info, but is not really necessary,
        // as a quick description similar to this should be available in the description of the parameter in the C3D file.
        string Meaning0;
        string Meaning1;
        string Meaning2;
        string Meaning3;
        string Meaning4;
        string Meaning5;
        string Meaning6;
        string Meaning7;


        // TODO: Finish this
        // For scalar and when ceebs
        protected SupportedParameter(string name, string description="") { }
        protected SupportedParameter(
            string name, string description,
            int dim0, string meaning1 = ""
            ) { }
        protected SupportedParameter(
            string name,
            int dim0, int dim1, 
            string meaning1 = "", string meaning2 = "",
            string description = ""
            ) { }
        protected SupportedParameter(
            string name,
            int dim0, int dim1, int dim2,
            string meaning1 = "", string meaning2 = "", string meaning3 = "",
            string description = ""
            ) { }
        protected SupportedParameter(
            string name,
            int dim0, int dim1, int dim2, int dim3,
            string meaning1 = "", string meaning2 = "", string meaning3 = "", string meaning4 = "",
            string description = ""
            ) { }
        protected SupportedParameter(

            int dim0, int dim1, int dim2, int dim3, int dim4,
            string meaning1 = "", string meaning2 = "", string meaning3 = "", string meaning4 = "", string meaning5 = "",
            string description = ""
            ) { }
        protected SupportedParameter(
            int dim0, int dim1, int dim2, int dim3, int dim4, int dim5,
            string meaning1 = "", string meaning2 = "", string meaning3 = "", string meaning4 = "", string meaning5 = "", string meaning6 = "",
            string description = ""
            ) { }
        protected SupportedParameter(
            int dim0, int dim1, int dim2, int dim3, int dim4, int dim5, int dim6,
            string meaning1 = "", string meaning2 = "", string meaning3 = "", string meaning4 = "", string meaning5 = "", string meaning6 = "", string meaning7 = "",
            string description = ""
            ) { }


    }
}
