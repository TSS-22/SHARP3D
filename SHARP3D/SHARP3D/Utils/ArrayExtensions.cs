using System.Linq;

namespace SHARP3D.Utils
{
    public static class ArrayExtensions
    {
        public static (T[] FlattenedArray, int[] OriginalDimensions) FlattenWithDimensions<T>(this Array array)
        {
            // Get the original dimensions (shape) of the array
            int[] dimensions = new int[array.Rank];
            for (int i = 0; i < array.Rank; i++)
            {
                dimensions[i] = array.GetLength(i);
            }

            // Flatten the array
            T[] flattenedArray = array.Cast<object>()
                                      .SelectMany<object, T>(x => x is Array ? Flatten<T>((Array)x) : new T[] { (T)x })
                                      .ToArray();

            return (flattenedArray, dimensions);
        }

        // Helper method to recursively flatten arrays
        private static T[] Flatten<T>(this Array array)
        {
            return array.Cast<object>()
                       .SelectMany<object, T>(x => x is Array ? Flatten<T>((Array)x) : new T[] { (T)x })
                       .ToArray();
        }
    }
}
