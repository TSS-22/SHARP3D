using SHARP3D.Header.DataEntity;
using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Test.ToolKit;
using SHARP3D.Utils.Enum;
using System.Text.RegularExpressions;

namespace SHARP3D.Test.Tests
{
    public class IEquatableTests
    {
        // Goup variables
        public static readonly string Name = "Test_name";
        public static readonly string Description = "Blablabla";
        public static readonly int Id = 0;
        public static readonly int PointerNextParameterStruct = 20;
        public static readonly bool Locked = false;
        public static readonly List<C3dFileParameter> Parameters = new List<C3dFileParameter>();

        // Parameter variables
        public static readonly DataType DataTypeFile = DataType.FLOAT32;
        public static readonly int NbOfDimensions = 1;
        public static readonly Array IntArray1 = TestingTools.CreateIntArray(5, 1);
        public static readonly Array IntArray2 = TestingTools.CreateIntArray(5, 2);

        // Public static Array of float with different values
        public static readonly Array FloatArray1 = TestingTools.CreateFloatArray(5,1f);
        public static readonly Array FloatArray2 = TestingTools.CreateFloatArray(5,2f);

        [Fact]
        public void ParameterGroupEmptyEquality_Test()
        {
            C3dFileParameterGroup group1 = new C3dFileParameterGroup();
            C3dFileParameterGroup group2 = new C3dFileParameterGroup();

            Assert.True(TestingTools.AssertEqual(group1, group2));
        }

        [Fact]
        public void ParameterGroupEquality_Test()
        {
            C3dFileParameterGroup group1 = new C3dFileParameterGroup
            {
                NameLength = (sbyte)Name.Length,
                Id = Id,
                Name = Name,
                PointerNextParameterStruct = PointerNextParameterStruct,
                DescriptionLength = Description.Length,
                ActualDescriptionLength = Description.Length,
                Description = Description,
                Parameters = Parameters
            };

            C3dFileParameterGroup group2 = new C3dFileParameterGroup
            {
                NameLength = (sbyte)Name.Length,
                Id = Id,
                Name = Name,
                PointerNextParameterStruct = PointerNextParameterStruct,
                DescriptionLength = Description.Length,
                ActualDescriptionLength = Description.Length,
                Description = Description,
                Parameters = Parameters
            };

            Assert.False(TestingTools.AssertInequal(group1, group2));
            Assert.True(TestingTools.AssertEqual(group1, group2));
        }

        [Fact]
        public void ParameterGroupInequality_Test()
        {
            C3dFileParameterGroup group1 = new C3dFileParameterGroup
            {
                NameLength = (sbyte)Name.Length,
                Id = Id,
                Name = Name,
                PointerNextParameterStruct = PointerNextParameterStruct,
                DescriptionLength = Description.Length,
                ActualDescriptionLength = Description.Length,
                Description = Description,
                Parameters = Parameters
            };

            List<C3dFileParameter> parametersDifferent = new List<C3dFileParameter>();
            parametersDifferent.Add(new C3dFileParameter());

            C3dFileParameterGroup group2 = new C3dFileParameterGroup
            {
                NameLength = (sbyte)Name.Length,
                Id = Id,
                Name = Name,
                PointerNextParameterStruct = PointerNextParameterStruct,
                DescriptionLength = Description.Length,
                ActualDescriptionLength = Description.Length,
                Description = Description,
                Parameters = parametersDifferent
            };

            Assert.True(TestingTools.AssertInequal(group1, group2));
            Assert.False(TestingTools.AssertEqual(group1, group2));
        }

        [Fact]
        public void ParameterEmptyEquality_Test()
        {
            C3dFileParameter param1 = new C3dFileParameter();
            Assert.False(TestingTools.AssertInequal(param1, param1));
            Assert.True(TestingTools.AssertEqual(param1, param1));
        }

        [Fact]
        public void ParameterEquality_Test()
        {
            C3dFileParameter param1 = new C3dFileParameter {
                NameLength = (sbyte)Name.Length,
                Id = Id,
                Name = Name,
                PointerNextParameterStruct = PointerNextParameterStruct,
                DescriptionLength = Description.Length,
                Description = Description,
                Data = IntArray1
            };

            C3dFileParameter param2 = new C3dFileParameter
            {
                NameLength = (sbyte)Name.Length,
                Id = Id,
                Name = Name,
                PointerNextParameterStruct = PointerNextParameterStruct,
                DescriptionLength = Description.Length,
                Description = Description,
                Data = IntArray1
            };
            Assert.False(TestingTools.AssertInequal(param1, param2));
            Assert.True(TestingTools.AssertEqual(param1, param2));
        }

        [Fact]
        public void ParameterInequality_Test1()
        {
            C3dFileParameter param1 = new C3dFileParameter
            {
                Data = IntArray1
            };

            C3dFileParameter param2 = new C3dFileParameter
            {
                Data = IntArray2
            };
            Assert.False(TestingTools.AssertEqual(param1, param2));
            Assert.True(TestingTools.AssertInequal(param1, param2));
        }

        [Fact]
        public void ParameterInequality_Test2()
        {
            C3dFileParameter param1 = new C3dFileParameter
            {
                Data = IntArray1
            };

            C3dFileParameter param2 = new C3dFileParameter
            {
                Data = FloatArray1
            };
            Assert.False(TestingTools.AssertEqual(param1, param2));
            Assert.True(TestingTools.AssertInequal(param1, param2));
        }

        [Fact]
        public void C3dHeaderEmptyEquality_Test()
        {
            C3dFileHeader header1 = new C3dFileHeader { };
            Assert.False(TestingTools.AssertInequal(header1, header1));
            Assert.True(TestingTools.AssertEqual(header1, header1));
        }

        [Fact]
        public void C3dHeaderInequality_Test()
        {
            C3dFileHeader header1 = new C3dFileHeader
            {
                Events = new C3dFileHeaderEvent[]
                {
                    new C3dFileHeaderEvent()
                }
            };

            C3dFileHeader header2 = new C3dFileHeader
            {
                Events = new C3dFileHeaderEvent[]
                {
                    new C3dFileHeaderEvent
                    {
                        EventTime = 10f
                    }
                }
            };

            Assert.True(TestingTools.AssertInequal(header1, header2));
            Assert.False(TestingTools.AssertEqual(header1, header2));
        }

    }
}
