using SHARP3D.Header;
using SHARP3D.Parameter.Data;
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
        public static readonly List<C3dParameter> Parameters = new List<C3dParameter>();

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
            C3dParameterGroup group1 = new C3dParameterGroup();
            C3dParameterGroup group2 = new C3dParameterGroup();

            Assert.True(TestingTools.AssertEqual(group1, group2));
        }

        [Fact]
        public void ParameterGroupEquality_Test()
        {
            C3dParameterGroup group1 = new C3dParameterGroup
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

            C3dParameterGroup group2 = new C3dParameterGroup
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
            C3dParameterGroup group1 = new C3dParameterGroup
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

            List<C3dParameter> parametersDifferent = new List<C3dParameter>();
            parametersDifferent.Add(new C3dParameter());

            C3dParameterGroup group2 = new C3dParameterGroup
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
            C3dParameter param1 = new C3dParameter();
            Assert.False(TestingTools.AssertInequal(param1, param1));
            Assert.True(TestingTools.AssertEqual(param1, param1));
        }

        [Fact]
        public void ParameterEquality_Test()
        {
            C3dParameter param1 = new C3dParameter {
                NameLength = (sbyte)Name.Length,
                Id = Id,
                Name = Name,
                PointerNextParameterStruct = PointerNextParameterStruct,
                DescriptionLength = Description.Length,
                Description = Description,
                Data = IntArray1
            };

            C3dParameter param2 = new C3dParameter
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
            C3dParameter param1 = new C3dParameter
            {
                Data = IntArray1
            };

            C3dParameter param2 = new C3dParameter
            {
                Data = IntArray2
            };
            Assert.False(TestingTools.AssertEqual(param1, param2));
            Assert.True(TestingTools.AssertInequal(param1, param2));
        }

        [Fact]
        public void ParameterInequality_Test2()
        {
            C3dParameter param1 = new C3dParameter
            {
                Data = IntArray1
            };

            C3dParameter param2 = new C3dParameter
            {
                Data = FloatArray1
            };
            Assert.False(TestingTools.AssertEqual(param1, param2));
            Assert.True(TestingTools.AssertInequal(param1, param2));
        }

        [Fact]
        public void C3dHeaderEmptyEquality_Test()
        {
            C3dHeader header1 = new C3dHeader { };
            Assert.False(TestingTools.AssertInequal(header1, header1));
            Assert.True(TestingTools.AssertEqual(header1, header1));
        }

        [Fact]
        public void C3dHeaderInequality_Test()
        {
            C3dHeader header1 = new C3dHeader
            {
                Events = new C3dHeaderEvent[]
                {
                    new C3dHeaderEvent()
                }
            };

            C3dHeader header2 = new C3dHeader
            {
                Events = new C3dHeaderEvent[]
                {
                    new C3dHeaderEvent
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
