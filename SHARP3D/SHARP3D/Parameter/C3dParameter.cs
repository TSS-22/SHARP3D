using SHARP3D.Parameter.ParameterDataType;
using SHARP3D.Utils.Enum;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SHARP3D.Parameter
{
    public struct C3dParameter
    {
        public sbyte NameLength;
        public int Id;
        public string Name;
        public int PointerNextParameterStruct;
        public DataLength DataType;
        public int NbOfDimensions;
        public int[]? Dimensions; // Do int[1] for scalar so it is consistent qith multidimensionnal.
        public ParameterData Data; // Doing this because it gives me shit with the abstract class
        public int DescriptionLength;
        public string Description;
        public bool Locked;


        public static C3dParameter FromBinaries(
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
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new CharParameterData(dataBytes),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };

                        case DataLength.BYTE:
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new ByteParameterData(dataBytes),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };

                        case DataLength.INT16:
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new IntParameterData(dataBytes),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };

                        case DataLength.FLOAT32:
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new FloatParameterData(dataBytes),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };
                        default:
                            throw new Exception($"Unknown data type {dataType}");
                    }
                default:
                    switch (dataType)
                    {
                        case DataLength.CHAR:
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new MultiCharParameterData(dataBytes, dimensions),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };
                        case DataLength.BYTE:
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new MultiByteParameterData(dataBytes, dimensions),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };
                        case DataLength.INT16:
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new MultiIntParameterData(dataBytes, dimensions, processor),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };
                        case DataLength.FLOAT32:
                            return new C3dParameter
                            {
                                NameLength = nameLength,
                                Id = id,
                                Name = name,
                                PointerNextParameterStruct = pointerNextParameterStruct,
                                DataType = dataType,
                                NbOfDimensions = nbOfDimensions,
                                Dimensions = dimensions,
                                Data = new MultiFloatParameterData(dataBytes, dimensions, processor),
                                DescriptionLength = descriptionLength,
                                Description = description,
                                Locked = locked
                            };
                        default:
                            throw new Exception($"Unknown data type {dataType}");
                    }

            }
        }
    }
}

