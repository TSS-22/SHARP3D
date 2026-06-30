using SHARP3D.Parameter.DataEntity.File;
using SHARP3D.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
        public void DeleteUneededParametersFromFiles()
        {
            List<C3dParameterGroup> newGroups = new List<C3dParameterGroup>();
            for(int idGroup = 0; idGroup<Groups.Count; idGroup++)
            {
                string[] regexParametersList = new string[] { };
                if (Sharp3dConstants.ParameterToDiscardFromC3dFileToC3d.TryGetValue(Groups[idGroup].Name, out regexParametersList))
                {
                    List<C3dParameter> newParameters = new List<C3dParameter>();
                    for(int idParameter = 0; idParameter < Groups[idGroup].Parameters.Count; idParameter++)
                    {
                        bool toKeep = true;
                        foreach(string regexParameter in regexParametersList)
                        {
                            if (Regex.IsMatch(Groups[idGroup].Parameters[idParameter].Name, regexParameter))
                            {
                                toKeep = false;
                                break;
                            }
                        }
                        if (toKeep)
                        {
                            newParameters.Add(Groups[idGroup].Parameters[idParameter]);
                        }
                    }
                    Groups[idGroup].Parameters = newParameters;
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

        public int GetGroupIndex(string groupName)
        {
            for(int i=0; i<Groups.Count; i++)
            {
                if (Groups[i].Name == groupName.ToUpper())
                {
                    return i;
                }
            }
            throw new ArgumentException($"Parameter group with name '{groupName.ToUpper()}' not found in section.");
        }

        public void AddGroup(string name, string description, List<C3dParameter>? parameters = null)
        {
            foreach(C3dParameterGroup group in Groups)
            {
                if ((group.Name == name.ToUpper()))
                {
                    throw new ArgumentException($"A parameter group with the name '{name.ToUpper()}' already exists in section.");
                }
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

        public (int,int) GetParameterIndex(string groupName, string parameterName)
        {
            int groupIndex = GetGroupIndex(groupName);
            return (groupIndex, Groups[groupIndex].GetParameterIndex(parameterName));
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

        public string DisplayStringParameterTree()
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

        public string DisplayStringListParameters(C3dParameterGroup group)
        {
            string parameters = $"------\n{group.Name}:\n------\n";

            foreach (C3dParameter parameter in group.Parameters)
            {
                parameters = string.Concat(parameters, $"{parameter.Name}\n");
            }
            parameters = string.Concat(parameters, "------\n");

            return parameters;
        }

        public string DisplayStringListParameters(string groupName)
        {
            try
            {
                return DisplayStringListParameters(GetGroup(groupName));
            }
            catch (ArgumentException ex) 
            {
                throw;
            }
            
        }

        public string DisplayStringListGroups()
        {
            string parameterTree = "------\nGroups:\n------\n";

            foreach (C3dParameterGroup group in Groups)
            {
                parameterTree = string.Concat(parameterTree, $"{group.Name}\n"); 
            }
            parameterTree = string.Concat(parameterTree, "------\n");
            return parameterTree;
        }

        public Dictionary<string, string[]> GetStringParameterTree()
        {
            Dictionary<string, string[]> parameterTree = new Dictionary<string, string[]>();

            foreach (C3dParameterGroup group in Groups)
            {
                parameterTree.Add(group.Name, GetStringListParameters(group.Name));
            }

            return parameterTree;
        }

        public string[] GetStringListParameters(C3dParameterGroup group)
        {
            List<string> parametersNameList = new List<string>();
            foreach (C3dParameter parameter in group.Parameters)
            {
                parametersNameList.Add(parameter.Name);
            }
            return parametersNameList.ToArray();
        }

        public string[] GetStringListParameters(string groupName)
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

        public string[] GetStringListGroups()
        {
            List<string> groupsNameList = new List<string>();
            foreach (C3dParameterGroup group in Groups)
            {
                groupsNameList.Add(group.Name);
            }
            return groupsNameList.ToArray();
        }

    }
}
