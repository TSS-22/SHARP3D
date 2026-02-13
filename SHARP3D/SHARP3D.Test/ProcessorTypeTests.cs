using SHARP3D.Utils.Enum;
using Xunit;

namespace SHARP3D.Test
{
    public class ProcessorTypeTests
    {


        [Theory]
        [InlineData(84, ProcessorType.INTEL)]
        [InlineData(85, ProcessorType.DEC)]
        [InlineData(86, ProcessorType.SIG_MIPS)]
        [InlineData(50, ProcessorType.UNKOWN)]
        [InlineData(-1, ProcessorType.UNKOWN)]
        public void FromInt_Returns_Correct_ProcessorType(int value, ProcessorType expected)
        {
            ProcessorType result = ProcessorTypeExtensions.FromInt(value);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData((byte)84, ProcessorType.INTEL)]
        [InlineData((byte)85, ProcessorType.DEC)]
        [InlineData((byte)86, ProcessorType.SIG_MIPS)]
        [InlineData(50, ProcessorType.UNKOWN)]
        public void FromByte_Returns_Correct_ProcessorType(byte value, ProcessorType expected)
        {
            ProcessorType result = ProcessorTypeExtensions.FromByte(value);
            Assert.Equal(expected, result);
        }

    }
}