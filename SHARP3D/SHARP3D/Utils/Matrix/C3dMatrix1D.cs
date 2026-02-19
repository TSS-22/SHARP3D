namespace SHARP3D.Utils.Matrix
{
    public class C3dMatrix1D<T>: C3dMatrix
    {
        List<T> Values { get; } = new List<T>();

        public C3dMatrix1D() { }
    }
}
