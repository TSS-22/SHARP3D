
using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Utils;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
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
                // Check if parameter name is required/reserved
                string[] regexParametersList = new string[] { };
                if (Sharp3dConstants.RegexParameterToDiscardFromC3dFileToC3d.TryGetValue(Group, out regexParametersList))
                {
                    foreach (string regexParameter in regexParametersList)
                    {
                        if (Regex.IsMatch(value, regexParameter))
                        {
                            throw new ArgumentException($"Parameter '{value}' cannot be created in this group {Group}. See documentation about reserved parameter and how to interact with them.");
                        }
                    }
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
            bool isFortranOrdered = false,
            string description= "No Description provided.",
            bool locked=false
            )
        {
            // Check data ranks
            if(data.Rank > 7)
            {
                throw new ArgumentException("C3d only support data array of up to 7 dimensions.");
            }
            // Check data type
            if (ArrayUtils.IsBaseElementPrimitive(data))
            {
                if (ArrayUtils.IsBaseElementString(data))
                {
                    Data = isFortranOrdered ? StringArrayToPaddedCharArray(data) : StringArrayToPaddedCharArray(data).ToFortranColumnMajor();
                }
                else
                {
                    Data = isFortranOrdered ? data : data.ToFortranColumnMajor();
                }
            }
            else
            {
                throw new ArgumentException($"Only primitive types and string are supported. {ArrayUtils.GetTypeBaseElement(data)} is unsupported.");
            }
            Group = group.ToUpper();
            Name = name.ToUpper();
            Description = description;
            Dimensions = GetDimensions(data);
            Locked = locked;
        }

        public C3dParameter(string groupName, C3dFileParameter parameter)
        {
            // Check data ranks
            if (parameter.Data.Rank > 7)
            {
                throw new ArgumentException("C3d only support data array of up to 7 dimensions.");
            }
            // Check data type
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
                throw new ArgumentException($"Only primitive types and string are supported. {ArrayUtils.GetTypeBaseElement(parameter.Data)} is unsupported.");
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
