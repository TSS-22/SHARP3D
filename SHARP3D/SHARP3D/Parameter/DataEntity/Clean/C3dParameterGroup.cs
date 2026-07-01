using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Utils;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SHARP3D.Parameter.DataEntity.Clean
{
    public class C3dParameterGroup
    {
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
        public List<C3dParameter> Parameters = new List<C3dParameter> { };
        bool Locked = false;

        public C3dParameterGroup(string name, string description, List<C3dParameter>? parameters = null, bool locked= false)
        {
            Name = name.ToUpper();
            Description = description;
            if(parameters != null)
            {
                Parameters = parameters;
            }
        }

        public C3dParameterGroup(C3dFileParameterGroup group)
        {
            Name = group.Name.ToUpper();
            Description = group.Description;
            Locked = group.Locked;
            foreach (C3dFileParameter parameter in group.Parameters)
            {
                AddParameter(parameter);
            }
        }

        public string[] ListParametersName()
        {
            string[] parameterNames = new string[Parameters.Count];
            for (int i = 0; i < Parameters.Count; i++)
            {
                parameterNames[i] = Parameters[i].Name;
            }
            return parameterNames;
        }

        public C3dParameter GetParameter(string name)
        {
            foreach (C3dParameter parameter in Parameters)
            {
                if (parameter.Name == name.ToUpper())
                {
                    return parameter;
                }
            }
            throw new ArgumentException($"Parameter with name '{name.ToUpper()}' not found in group '{Name}'.");
        }

        public int GetParameterIndex(string nameParameter)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Name == nameParameter.ToUpper())
                {
                    return i;
                }
            }
            throw new ArgumentException($"Parameter with name '{nameParameter.ToUpper()}' not found in group '{Name}'.");            
        }

        public void AddParameter(string parameterName, Array data, string description= "No Description provided.", bool locked=false, bool isFortranOrdered = false)
        {
            if (Parameters.Any(p => p.Name == parameterName.ToUpper()))
            {
                throw new ArgumentException($"A parameter with the name '{parameterName}' already exists in group '{Name}'.");
            }
            // Check if the parameter as a taken name
            string[] regexParametersList = new string[] { };
            if (Sharp3dConstants.RegexParameterToDiscardFromC3dFileToC3d.TryGetValue(Name, out regexParametersList))
            {
                foreach (string regexParameter in regexParametersList)
                {
                    if (Regex.IsMatch(parameterName, regexParameter))
                    {
                        throw new ArgumentException($"Parameter '{parameterName}' cannot be created in group {Name}. See documentation about reserved parameter and how to interact with them.");
                    }
                }
            }

            Parameters.Add(new C3dParameter(Name, parameterName, data, isFortranOrdered, description));
        }

        public void AddParameter(C3dFileParameter fileParameter)
        {
            if (Parameters.Any(p => p.Name == fileParameter.Name.ToUpper()))
            {
                throw new ArgumentException($"A parameter with the name '{fileParameter.Name}' already exists in group '{Name}'.");
            }
            // Check if the parameter as a taken name
            string[] regexParametersList = new string[] { };
            if (Sharp3dConstants.RegexParameterToDiscardFromC3dFileToC3d.TryGetValue(Name, out regexParametersList))
            {
                foreach (string regexParameter in regexParametersList)
                {
                    if (Regex.IsMatch(fileParameter.Name, regexParameter))
                    {
                        throw new ArgumentException($"Parameter '{fileParameter.Name}' cannot be created in group {Name}. See documentation about reserved parameter and how to interact with them.");
                    }
                }
            }
            Parameters.Add(new C3dParameter(Name, fileParameter));
        }

        public void DeleteParameter(string parameterName)
        {
            // Check if the parameter as a taken name
            string[] regexParametersList = new string[] { };
            if (Sharp3dConstants.RegexParameterToDiscardFromC3dFileToC3d.TryGetValue(Name, out regexParametersList))
            {
                foreach (string regexParameter in regexParametersList)
                {
                    if (Regex.IsMatch(parameterName, regexParameter))
                    {
                        throw new ArgumentException($"Parameter '{parameterName}' cannot be deleted in group {Name}. See documentation about reserved parameter and how to interact with them.");
                    }
                }
            }
            bool removed = Parameters.RemoveAll(p => p.Name == parameterName.ToUpper()) > 0;
            if (!removed)
            {
                throw new ArgumentException($"Parameter with name '{parameterName.ToUpper()}' not found in group '{Name}'.");
            }
        }
    }
}
