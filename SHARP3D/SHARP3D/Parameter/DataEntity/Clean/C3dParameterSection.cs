namespace SHARP3D.Parameter.DataEntity.Clean
{
    public class C3dParameterSection
    {
         List<C3dParameterGroup> Groups = new List<C3dParameterGroup> { };

        public C3dParameterSection() { }

        public C3dParameterSection(List<C3dParameterGroup> parameterGroups)
        {
            Groups = parameterGroups;
        }

        public C3dParameterSection(List<C3dFileParameterGroup> fileGroups)
        {
            List<C3dParameterGroup> groups = new List<C3dParameterGroup>();
            foreach (C3dFileParameterGroup fileGroup in fileGroups)
            {
                Console.WriteLine(fileGroup.Name);
                groups.Add(new C3dParameterGroup(fileGroup));
            }

            Groups = groups;
        }

        public C3dParameterGroup GetGroup(string name)
        {
            foreach (C3dParameterGroup group in Groups)
            {
                if (group.Name == name.ToUpper())
                {
                    return group;
                }
            }
            throw new ArgumentException($"Parameter group with name '{name.ToUpper()}' not found in section.");
        }
        public void AddGroup(string name, string description, List<C3dParameter>? parameters = null)
        {
            if (Groups.Any(g => g.Name == name.ToUpper()))
            {
                throw new ArgumentException($"A parameter group with the name '{name.ToUpper()}' already exists in section.");
            }
            Groups.Add(new C3dParameterGroup(name, description, parameters));
        }


        public void DeleteGroup(string groupName)
        {
            bool removed = Groups.RemoveAll(p => p.Name == groupName.ToUpper()) > 0;
            if (!removed)
            {
                throw new ArgumentException($"Group '{groupName.ToUpper()}' was not found.");
            }
        }

        public C3dParameter[] GetParameter(string parameterName)
        {
            List<C3dParameter> parametersFound = new List<C3dParameter>();
            foreach (C3dParameterGroup group in Groups)
            {
                try
                {
                    parametersFound.Add(group.GetParameter(parameterName));
                }
                catch(ArgumentException ex)
                {
                    // No parameter with NAME in GROUP
                }
            }

            if (parametersFound.Count == 0)
            {
                throw new ArgumentException($"No parameter {parameterName.ToUpper()} found in the Parameter Section.");
            }

            return parametersFound.ToArray();
        }

        public C3dParameter GetParameter(string groupName, string parameterName)
        {
            C3dParameterGroup groupFound = GetGroup(groupName);
            return groupFound.GetParameter(parameterName);
        }

        public void DeleteParameter(string groupName, string parameterName)
        {
            C3dParameterGroup groupFound = GetGroup(groupName);
            bool removed = groupFound.Parameters.RemoveAll(p => p.Name == parameterName.ToUpper()) > 0;
            if (!removed)
            {
                throw new ArgumentException($"Parameter '{groupName.ToUpper()}:{parameterName.ToUpper()}' was not deleted because it cannot be found.");
            }
        }
    }
}
