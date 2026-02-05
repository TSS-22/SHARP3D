using System.IO;
using Xunit;

namespace SHARP3D.Test
{
	public class C3dFileManagerTests
	{
	
        [Theory]
        [InlineData(@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d", ProcessorType.INTEL)]
        [InlineData(@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pr.c3d", ProcessorType.INTEL)]
        [InlineData(@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015si.c3d", ProcessorType.SIG_MIPS)]
        [InlineData(@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015sr.c3d", ProcessorType.SIG_MIPS)]
        [InlineData(@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vi.c3d", ProcessorType.DEC)]
        [InlineData(@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vr.c3d", ProcessorType.DEC)]
	    public void Get_Pointer_To_Parameter_Section(string filePath, ProcessorType expectedProcessorType)
        {
    	    using (FileStream fs = new FileStream(filePath, FileMode.Open))
			    {
      	    ProcessorType processorByte = C3dFileManager.ReadProcessorByte(fs);
            Assert.Equal(expectedProcessorType, processorByte);
          }
        }

	}
}