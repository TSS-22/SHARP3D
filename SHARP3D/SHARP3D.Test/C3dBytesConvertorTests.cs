using Xunit;

namespace SHARP3D.Test
{
    public class C3dBytesConvertorTests
    {
        [Theory]
        [InlineData(450, ProcessorType.DEC)]
        [InlineData(450, ProcessorType.INTEL)]
        [InlineData(450, ProcessorType.SIG_MIPS)]
        public void Bytes_To_Int(int value, ProcessorType processorType)
        {
            int calculatedValue = C3dBytesConvertor.ToInt(BitConverter.GetBytes(value), processorType);
            Assert.Equal(value, calculatedValue);
        }

        [Theory]
        [InlineData(450, ProcessorType.DEC)]
        [InlineData(450, ProcessorType.INTEL)]
        [InlineData(450, ProcessorType.SIG_MIPS)]
        public void Int_To_Bytes(int value, ProcessorType processorType)
        {
            byte[] calculatedValue = C3dBytesConvertor.ToBytes(value, processorType);
            Assert.Equal(BitConverter.GetBytes(value), calculatedValue);
        }

        [Theory]
        [InlineData(50.0, ProcessorType.DEC)]
        [InlineData(50.0, ProcessorType.INTEL)]
        [InlineData(50.0, ProcessorType.SIG_MIPS)]
        public void Bytes_To_Float(float value, ProcessorType processorType)
        {
            float calculatedValue = C3dBytesConvertor.ToFloat(BitConverter.GetBytes(value), processorType);
            Assert.Equal(value, calculatedValue);
        }

        [Theory]
        [InlineData(50.0, ProcessorType.DEC)]
        [InlineData(50.0, ProcessorType.INTEL)]
        [InlineData(50.0, ProcessorType.SIG_MIPS)]
        public void Float_To_Bytes(float value, ProcessorType processorType)
        {
            byte[] calculatedValue = C3dBytesConvertor.ToBytes(value, processorType);
            Assert.Equal(BitConverter.GetBytes(value), calculatedValue);
        }
    }
}
