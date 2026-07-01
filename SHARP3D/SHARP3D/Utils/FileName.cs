namespace SHARP3D.Utils
{
    public class StringArrayPadder
    {
        // Convert string array of any dimensionality (1-6D) to char array (+1 dim for characters)
        public static Array PadStringsToChar<T>(Array inputArray, int? maxStringLength = null)
        {
            if (inputArray == null) throw new ArgumentNullException(nameof(inputArray));

            var rank = inputArray.Rank;

            // Validate dimensionality constraint
            if (rank > 6 || rank < 1)
                throw new ArgumentException($"Input must be 1-6 dimensional, got {rank}");

            // Find all indices first
            var indexes = GetAllIndexes(inputArray);

            // Determine max string length
            int maxLength = maxStringLength ?? indexes.Max(idx =>
            {
                var value = inputArray.GetValue(idx) as string;
                return value?.Length ?? 0;
            });

            // Create output char array with additional dimension for characters
            int[] outputRanks = new int[rank + 1];
            for (int i = 0; i < rank; i++)
                outputRanks[i] = inputArray.GetLength(i);
            outputRanks[rank] = maxLength;

            var outputType = typeof(char);
            var outputArray = Array.CreateInstance(outputType, outputRanks);

            // Fill output array with space characters (ASCII 32)
            foreach (var idx in GetIndices(outputRanks))
                outputArray.SetValue(' ', idx);

            // Copy strings with padding
            foreach (var srcIdx in indexes)
            {
                var strVal = inputArray.GetValue(srcIdx) as string ?? "";
                var dstIdx = srcIdx.Concat(new[] { 0 }).ToArray();

                for (int i = 0; i < Math.Min(strVal.Length, maxLength); i++)
                    outputArray.SetValue(strVal[i], srcIdx.Concat(new[] { i }).ToArray());
            }

            return outputArray;
        }

        private static IEnumerable<int[]> GetAllIndexes(Array arr)
        {
            return FromRank(arr.Rank, arr.GetLength(0),
                Enumerable.Range(1, arr.Rank - 1).Select(r => arr.GetLength(r)).ToArray());
        }

        private static IEnumerable<int[]> FromRank(int remainingRanks, int currentSize, int[] nextSizes)
        {
            if (remainingRanks == 0) yield return new int[0];
            else
            {
                for (int i = 0; i < currentSize; i++)
                    foreach (var rest in FromRank(remainingRanks - 1, nextSizes.Length > 0 ? nextSizes[0] : 1,
                        nextSizes.Length > 1 ? nextSizes.Skip(1).ToArray() : new int[0]))
                        yield return new[] { i }.Concat(rest).ToArray();
            }
        }

        private static IEnumerable<int[]> GetIndices(int[] sizes)
        {
            if (sizes.Length == 0) yield return new int[0];
            else
            {
                for (int i = 0; i < sizes[0]; i++)
                    foreach (var rest in GetIndices(sizes.TakeLast(sizes.Length - 1).ToArray()))
                        yield return new[] { i }.Concat(rest).ToArray();
            }
        }
    }
}
