using SHARP3D.Exceptions;

namespace SHARP3D.Parameter.Data
{
    public class C3dParameterCollection
    {
        private readonly Dictionary<string, (string, int)[]> _groupValuesByName = new();
        private readonly Dictionary<int, (string, int)[]> _groupValuesByIndex = new();
        private readonly Dictionary<int, string> _mapGroupIndexToString = new();
        private readonly Dictionary<string, int> _mapGroupStringToIndex = new();

        public C3dParameterCollection() { }

        public C3dParameterCollection(List<C3dParameterGroup> c3dFileParameters) 
        {
            for(int i =0; i < c3dFileParameters.Count; i++)
            {
                _mapGroupIndexToString.Add(i, c3dFileParameters[i].Name);
                _mapGroupStringToIndex.Add(c3dFileParameters[i].Name, i);

                List<(string, int)> tempParameterValues = new List<(string, int)>();

                for (int j = 0; j < c3dFileParameters[i].Parameters.Count; j++) 
                {
                    tempParameterValues.Add((c3dFileParameters[i].Parameters[j].Name, j));
                }
                _groupValuesByName.Add(
                    c3dFileParameters[i].Name,
                    tempParameterValues.ToArray()
                    );
                _groupValuesByIndex.Add(i, tempParameterValues.ToArray());
            }
        }

        // TODO: All that + delete
        //public void AddParameter(string name, int index, object data)
        //{
        //    Dictionary<string, uint> parameter = new  (name, index, data);
        //    _byName.Add(name, parameter);
        //    _byIndex.Add(index, parameter);
        //}

        //public void AddGroup(string groupName, int groupIndex)
        //{
        //    if
        //}

        //public void AddParameterToGroup(string groupName, string parameterName, uint parameterIndex) { }

        public (int, int) GetParameterIndex(string groupName, string parameterName)
        {
            groupName = groupName.ToUpper();
            parameterName = parameterName.ToUpper();
            if (_groupValuesByName.TryGetValue(groupName, out (string, int)[] groupValues))
            {
                if ((groupValues.Length == 0) || groupValues == null) 
                {
                    throw new EmptyParameterGroupException($"The group \"{groupName}\" does not contain any parameters.");
                }
                foreach (var item in groupValues)
                {
                    if (item.Item1 == parameterName)
                    {
                        return (_mapGroupStringToIndex[groupName], item.Item2);
                    }
                }
                throw new ParameterNotFoundException($"The parameter \"{parameterName}\" was not found in the group \"{groupName}\"");
            }
            else 
            {
                throw new KeyNotFoundException($"The combination \"{groupName}\":\"{parameterName}\" don't exist.");
            }
        }

        public (string, int)[] ListGroupParameters(string groupName) 
        {
            groupName = groupName.ToUpper();
            if (_groupValuesByName.TryGetValue(groupName, out (string, int)[]? values))
            {
                if ((values.Length == 0) || values == null) 
                {
                    throw new EmptyParameterGroupException($"The group \"{groupName}\" does not contain any parameters.");
                }
                return (values);
            }
            else { throw new KeyNotFoundException($"The group \"{groupName}\" doesn't exist."); }
        }

        public string[] ListGroups()
        {
            return _groupValuesByName.Keys.ToArray();
        }
    }

}
