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

        public C3dParameterGroup(string name, string description, List<C3dParameter>? parameters = null)
        {
            Name = name;
            Description = description;
            if(parameters != null)
            {
                Parameters = parameters;
            }
        }

        public string[] ListParameters()
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
                if (parameter.Name == name)
                {
                    return parameter;
                }
            }
            throw new ArgumentException($"Parameter with name '{name.ToUpper()}' not found in group '{Name}'.");
        }

        public void AddParameter(string parameterName, string description, Array data)
        {
            if (Parameters.Any(p => p.Name == parameterName))
            {
                throw new ArgumentException($"A parameter with the name '{parameterName}' already exists in group '{Name}'.");
            }

            Parameters.Add(new C3dParameter(Name, parameterName, description, data));
        }

        public void DeleteParameter(string name)
        {
            bool removed = Parameters.RemoveAll(p => p.Name == name) > 0;
            if (!removed)
            {
                throw new ArgumentException($"Parameter with name '{name.ToUpper()}' not found in group '{Name}'.");
            }
        }
    }
}
