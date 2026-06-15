using System.Xml.Linq;

namespace SHARP3D.Parameter.DataEntity.Clean
{
    public class C3dParameterSection
    {
         List<C3dParameterGroup> Groups = new List<C3dParameterGroup> { };

        public C3dParameterSection(List<C3dParameterGroup> parameterGroups)
        {
            Groups = parameterGroups;
        }

        public C3dParameterGroup GetGroup(string name)
        {
            foreach (C3dParameterGroup group in Groups)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            throw new ArgumentException($"Parameter group with name '{name.ToUpper()}' not found in section.");
        }
        public void AddGroup(string name, string description, List<C3dParameter>? parameters = null)
        {
            if (Groups.Any(g => g.Name == name))
            {
                throw new ArgumentException($"A parameter group with the name '{name}' already exists in section.");
            }
            Groups.Add(new C3dParameterGroup(name, description, parameters));
        }


        public void DeleteGroup(string groupName)
        {
            bool removed = Groups.RemoveAll(p => p.Name == groupName) > 0;
            if (!removed)
            {
                throw new ArgumentException($"Group '{groupName}' was not found.");
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
    }
}
