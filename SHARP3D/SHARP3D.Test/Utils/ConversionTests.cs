using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHARP3D.Test.Utils
{
    public class ConversionTests
    {
        public static readonly int[] val0 = new int[] { -32768, 32768 };
        public static readonly int[] val1 = new int[] { -1, 65535 };
        public static readonly int[] val2 = new int[] { 0, 0 };
        public static readonly int[] val3 = new int[] { 32767, 32767 };
        public static readonly int[] val4 = new int[] { 65535, 65535 };
        public static readonly int[] val5 = new int[] { -2, 65534 };
        public static readonly int[] val6 = new int[] { 1, 1 };
        public static readonly int[] val7 = new int[] { 16384, 16384 };
        public static readonly int[] val8 = new int[] { -65535, 1 };
        public static readonly int[] val9 = new int[] { -65536, 0 };


        public static IEnumerable<object[]> IntValueData =>
            new List<object[]>
            {
                new object[] { val0},
                new object[] { val1},
                new object[] { val2},
                new object[] { val3},
                new object[] { val4},
                new object[] { val5},
                new object[] { val6},
                new object[] { val7},
                new object[] { val8},
                new object[] { val9},
            };

        [Theory]
        [MemberData(nameof(IntValueData))]
        public void ConversionSignedToUnsignedInt_Test(int[] values)
        {
            short signedData = (short)values[0];
            short unsignedData = (short)values[1];

            short valueToTest = (short)(signedData & 0xFFFF);

            Assert.Equal(unsignedData, valueToTest);
        }

    }
}
