using Xunit;

namespace SHARP3D.Test
{
    public class C3dBytesConvertorTests
    {
        public static readonly byte[] TestBytesInt_DEC_INTEL = { 0xC2, 0x01 };
        public static readonly byte[] TestBytesInt_SIGMIPS = { 0x01, 0xC2 };
        public static readonly byte[] TestBytesFloat_DEC = { 0x48, 0x43, 0x00, 0x00 };
        public static readonly byte[] TestBytesFloat_INTEL = { 0x00, 0x00, 0x48, 0x42 };
        public static readonly byte[] TestBytesFloat_SIGMIPS = { 0x48, 0x42, 0x00, 0x00 };

        public static IEnumerable<object[]> BytesToIntData =>
            new List<object[]>
            {
                new object[] { 450, TestBytesInt_DEC_INTEL, ProcessorType.DEC },
                new object[] { 450, TestBytesInt_DEC_INTEL, ProcessorType.INTEL },
                new object[] { 450, TestBytesInt_SIGMIPS, ProcessorType.SIG_MIPS },
            };

        public static IEnumerable<object[]> IntToBytesData =>
            new List<object[]>
            {
                new object[] { TestBytesInt_DEC_INTEL, 450, ProcessorType.DEC },
                new object[] { TestBytesInt_DEC_INTEL, 450, ProcessorType.INTEL },
                new object[] { TestBytesInt_SIGMIPS, 450, ProcessorType.SIG_MIPS },
            };

        public static IEnumerable<object[]> BytesToFloatData =>
            new List<object[]>
            {
                new object[] { 50.0, TestBytesFloat_DEC, ProcessorType.DEC },
                new object[] { 50.0, TestBytesFloat_INTEL, ProcessorType.INTEL },
                new object[] { 50.0, TestBytesFloat_SIGMIPS, ProcessorType.SIG_MIPS },
            };

        public static IEnumerable<object[]> FloatToBytesData =>
            new List<object[]>
            {
                new object[] { TestBytesFloat_DEC, 50.0, ProcessorType.DEC },
                new object[] { TestBytesFloat_INTEL, 50.0, ProcessorType.INTEL },
                new object[] { TestBytesFloat_SIGMIPS, 50.0, ProcessorType.SIG_MIPS },
            };

        [Theory]
        [MemberData(nameof(BytesToIntData))]
        public void Bytes_To_Int(int expectedValue, byte[] testValue, ProcessorType processorType)
        {
            
            int calculatedValue = C3dBytesConvertor.ToInt(testValue, processorType);
            Assert.Equal(expectedValue, calculatedValue);
        }

        [Theory]
        [MemberData(nameof(IntToBytesData))]
        public void Int_To_Bytes(byte[] expectedValue, int testValue, ProcessorType processorType)
        {
            byte[] calculatedValue = C3dBytesConvertor.ToBytes(testValue, processorType);
            Assert.Equal(expectedValue, calculatedValue);
        }

        [Theory]
        [MemberData(nameof(BytesToFloatData))]
        public void Bytes_To_Float(float expectedValue, byte[] testValue, ProcessorType processorType)
        {
            float calculatedValue = C3dBytesConvertor.ToFloat(testValue, processorType);
            Assert.Equal(expectedValue, calculatedValue);
        }

        [Theory]
        [MemberData(nameof(FloatToBytesData))]
        public void Float_To_Bytes(byte[] expectedValue, float testValue, ProcessorType processorType)
        {
            byte[] calculatedValue = C3dBytesConvertor.ToBytes(testValue, processorType);
            Assert.Equal(expectedValue, calculatedValue);
        }
    }
}
