namespace SHARP3D.Parameter.DataEntity.Clean
{
    public class C3dParameterSection
    {
         List<C3dParameterGroup> ParameterGroups = new List<C3dParameterGroup> { };

        public C3dParameterSection(List<C3dParameterGroup> parameterGroups)
        {
            ParameterGroups = parameterGroups;
        }
        public C3dParameterGroup GetParameterGroup(string name)
        {
            foreach (C3dParameterGroup group in ParameterGroups)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            throw new ArgumentException($"Parameter group with name '{name.ToUpper()}' not found in section.");
        }
        public void AddParameterGroup(string name, string description, List<C3dParameter> parameters)
        {
            if (ParameterGroups.Any(g => g.Name == name))
            {
                throw new ArgumentException($"A parameter group with the name '{name}' already exists in section.");
            }
            ParameterGroups.Add(new C3dParameterGroup(name, description, parameters));
        }

    }
}
