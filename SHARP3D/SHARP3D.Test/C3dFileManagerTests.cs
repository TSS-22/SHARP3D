using System.IO;
using Xunit;

namespace SHARP3D.Test
{
	public class C3dFileManagerTests
	{
		string[] paths_testSuite = {
				@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d",
				@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pr.c3d",
				@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015si.c3d",
				@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015sr.c3d",
				@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vi.c3d",
				@"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vr.c3d",
			};

		SHARP3D.ProcessorType[] expectedProcessorTypes = {
				ProcessorType.INTEL,
				ProcessorType.INTEL,
				ProcessorType.SIG_MIPS,
				ProcessorType.SIG_MIPS,
				ProcessorType.DEC,
				ProcessorType.DEC
			};


		[Fact]
		public void Get_Pointer_To_Parameter_Section()
		{
			for (int i = 0; i < paths_testSuite.Length; i++)
			{
				using (FileStream fs = new FileStream(paths_testSuite[i], FileMode.Open))
				{
					byte processorByte = C3dFileManager.ReadProcessorByte(fs);
					Assert.Equal((byte)expectedProcessorTypes[i], processorByte);
				}				
			}
		}
	}
}