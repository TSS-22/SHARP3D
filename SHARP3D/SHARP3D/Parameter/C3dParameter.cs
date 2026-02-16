using SHARP3D.Parameter.ParameterDataType;
using SHARP3D.Utils.Enum;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SHARP3D.Parameter
{
    public struct C3dParameter<T>
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public int PointerNextParameterStruct;
        public DataLength DataType;
        public int NbOfDimensions;
        public int[]? Dimensions; // Do int[1] for scalar so it is consistent qith multidimensionnal.
        public T Data; // Doing this because it gives me shit with the abstract class
        public int DescriptionLength;
        public string Description;
        public bool Locked;
    

        public static C3dParameter<T> FromBinaries(
                sbyte nameLength,
                sbyte id,
                string name,
                int pointerNextParameterStruct,
                int descriptionLength,
                string description,
                bool locked,
                DataLength dataType,
                int nbOfDimensions,
                int[]? dimensions,
                byte[] dataBytes,
                ProcessorType processor
                )
            {
            
                switch (nbOfDimensions) {
                    case 0:
                        //TODO: Scalar case
                        switch (dataType) 
                        {
                            case DataLength.CHAR:
                                CharParameterData scalar_char = new CharParameterData(dataBytes);
                                C3dParameter<char> test = new C3dParameter<char>();
                                test.NameLength = nameLength;
                                test.Id = id;
                                test.Name = name;
                                test.PointerNextParameterStruct = pointerNextParameterStruct;
                                test.DataType = dataType;
                                test.NbOfDimensions = nbOfDimensions;
                                test.Dimensions = dimensions;
                                test.Data = scalar_char.Data;
                                test.DescriptionLength = descriptionLength;
                                test.Description = description;
                                test.Locked = locked;
                                return test;
                                break;
                            case DataLength.BYTE:
                                ByteParameterData scalar_byte = new ByteParameterData(dataBytes);
                                break;
                            case DataLength.INT16:
                                IntParameterData scalar_int = new IntParameterData(dataBytes);
                                break;
                            case DataLength.FLOAT32:
                                FloatParameterData scalar_float = new FloatParameterData(dataBytes);
                                break;
                            default:
                                throw new Exception($"Unknown data type {dataType}");
                        }
                        break;
                    default:
                        break;
                        //TODO: Multidimensionnal case
                }            
            }

        private static T GetData(T test)
        {
            switch (test)
            {
                case int:
                    return 10;
                case float:
                    return 10.0f;
                // ...
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
