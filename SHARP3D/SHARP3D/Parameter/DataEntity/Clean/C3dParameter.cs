
using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SHARP3D.Parameter.DataEntity.Clean
{
    public class C3dParameter
    {
        public string Group;
        private string _name = "UNKNOWN";
        public string Name
        {
            get => _name; // Getter
            set
            {
                // Custom logic in the setter
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Parameter name cannot be null or whitespace.");
                }
                _name = value.ToUpper(); // Update the backing field
            }
        }
        public string Description = "No Description provided.";
        public int[] Dimensions { get; private set; }  = new int[] { };
        public Array _Data = new int[] { };
        public Array Data
        {
            get => _Data; // Getter
            set
            {
                // Custom logic in the setter
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentException("Parameter data cannot be empty or null.");
                }
                _Data = value;
                Dimensions = GetDimensions(value);
            }
        }
        public bool Locked = false;
    
        public C3dParameter(
            string group,
            string name,
            Array data,
            string description= "No Description provided.",
            bool locked=false
            )
        {
            Group = group.ToUpper();
            Name = name.ToUpper();
            Description = description;
            Data = data; // TODO
            Dimensions = GetDimensions(data);
            Locked = locked;
        }

        public C3dParameter(string groupName, C3dFileParameter parameter)
        {
            if (ArrayUtils.IsBaseElementPrimitive(parameter.Data))
            {
                if (ArrayUtils.IsBaseElementString(parameter.Data)) 
                {
                    Data = StringArrayToPaddedCharArray(parameter.Data);
                }
                else
                {
                    Data = parameter.Data;
                }    
            }
            else
            {
                throw new ArgumentException($"Only primitive types and string are supported. {parameter.Data.GetType().GetElementType().Name} is unsupported.");
            }
                Group = groupName.ToUpper();
            Name = parameter.Name.ToUpper();
            Description = parameter.Description;
            Dimensions = GetDimensions(Data);
            Locked = parameter.Locked;
        }

        private int[] GetDimensions(Array data)
        {
            List<int> dimensions = new List<int> { };
            for (int i = 0; i < Data.Rank; i++)
            {
                dimensions.Add(Data.GetLength(i));
            }
            return dimensions.ToArray();
        }

        private Array StringArrayToPaddedCharArray(Array stringData)
        {
            return new char[0];
        }

    }
}
