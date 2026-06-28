using SHARP3D.Exceptions;
using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

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
                groups.Add(new C3dParameterGroup(fileGroup));
            }

            Groups = groups;
        }
        public void CleanRequiredParameters()
        {
            // Discard from "Parameters" the required parameters
            foreach (KeyValuePair<string, string[]> group in Sharp3dConstants.ParameterToDiscardFromC3dFileToC3d)
            {
                foreach (string parameter in group.Value)
                {
                    try
                    {
                        DeleteParameter(group.Key, parameter);
                    }
                    catch (ArgumentException) { }
                }
            }
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

        public string GetStringParameterTree()
        {
            string parameterTree = "------\nGroups and Parameters:\n------\n";

            foreach(C3dParameterGroup group in Groups)
            {
                parameterTree = string.Concat(parameterTree, $"{group.Name}:\n");
                foreach(C3dParameter parameter in group.Parameters)
                {
                    parameterTree = string.Concat(parameterTree, $"\t{parameter.Name}\n");
                }
                parameterTree = string.Concat(parameterTree, "------\n");
            }

            return parameterTree;
        }

        public string GetStringListParameters(C3dParameterGroup group)
        {
            string parameters = $"------\n{group.Name}:\n------\n";

            foreach (C3dParameter parameter in group.Parameters)
            {
                parameters = string.Concat(parameters, $"{parameter.Name}\n");
            }
            parameters = string.Concat(parameters, "------\n");

            return parameters;
        }

        public string GetStringListParameters(string groupName)
        {
            try
            {
                return GetStringListParameters(GetGroup(groupName));
            }
            catch (ArgumentException ex) 
            {
                throw;
            }
            
        }

        public string GetStringListGroups()
        {
            string parameterTree = "------\nGroups:\n------\n";

            foreach (C3dParameterGroup group in Groups)
            {
                parameterTree = string.Concat(parameterTree, $"{group.Name}\n"); 
            }
            parameterTree = string.Concat(parameterTree, "------\n");
            return parameterTree;
        }

    }
}
