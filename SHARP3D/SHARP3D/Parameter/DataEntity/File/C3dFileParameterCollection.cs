using SHARP3D.Exceptions;

namespace SHARP3D.Parameter.DataEntity.File
{
    /// <summary>
    /// References all the groups and parameters in a two-way dictionary for convenience.
    /// </summary>
    /// <remarks>
    /// This class provides methods to retrieve parameter indices, list group parameters, and list all groups.
    /// </remarks>
    public class C3dFileParameterCollection
    {
        private readonly Dictionary<string, (string, int)[]> _groupValuesByName = new();
        private readonly Dictionary<int, (string, int)[]> _groupValuesByIndex = new();
        private readonly Dictionary<int, string> _mapGroupIndexToString = new();
        private readonly Dictionary<string, int> _mapGroupStringToIndex = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="C3dFileParameterCollection"/> class.
        /// </summary>
        public C3dFileParameterCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="C3dFileParameterCollection"/> class using a list of <see cref="C3dFileParameterGroup"/>.
        /// </summary>
        /// <param name="c3dFileParameters">The list of <see cref="C3dFileParameterGroup"/> to populate the collection.</param>
        public C3dFileParameterCollection(List<C3dFileParameterGroup> c3dFileParameters) 
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

        /// <summary>
        /// Gets the index of a parameter within a specified group.
        /// </summary>
        /// <param name="groupName">The name of the group to search in.</param>
        /// <param name="parameterName">The name of the parameter to find.</param>
        /// <returns>A tuple containing the group index and the parameter index.</returns>
        /// <exception cref="EmptyParameterGroupException">
        /// Thrown if the specified group does not contain any parameters.
        /// </exception>
        /// <exception cref="ParameterNotFoundException">
        /// Thrown if the specified parameter is not found in the group.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the specified group does not exist.
        /// </exception>
        public (int, int) GetParameterIndex(string groupName, string parameterName)
        {
            groupName = groupName.ToUpper();
            parameterName = parameterName.ToUpper();
            if (_groupValuesByName.TryGetValue(groupName, out (string, int)[] groupValues))
            {
                if (groupValues.Length == 0 || groupValues == null) 
                {
                    throw new EmptyParameterGroupException($"The group '{groupName}' does not contain any parameters.");
                }
                foreach (var item in groupValues)
                {
                    if (item.Item1 == parameterName)
                    {
                        return (_mapGroupStringToIndex[groupName], item.Item2);
                    }
                }
                throw new ParameterNotFoundException($"The parameter '{parameterName}' was not found in the group '{groupName}'");
            }
            else 
            {
                throw new ParameterNotFoundException($"The combination '{groupName}:{parameterName}' don't exist.");
            }
        }

        /// <summary>
        /// Lists all parameters in a specified group by group name.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>An array of tuples containing parameter names and their indices.</returns>
        /// <exception cref="EmptyParameterGroupException">
        /// Thrown if the specified group does not contain any parameters.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the specified group does not exist.
        /// </exception>
        public (string, int)[] ListGroupParameters(string groupName) 
        {
            groupName = groupName.ToUpper();
            if (_groupValuesByName.TryGetValue(groupName, out (string, int)[]? values))
            {
                if (values.Length == 0 || values == null) 
                {
                    throw new EmptyParameterGroupException($"The group \"{groupName}\" does not contain any parameters.");
                }
                return values;
            }
            else { throw new KeyNotFoundException($"The group \"{groupName}\" doesn't exist."); }
        }

        /// <summary>
        /// Lists all parameters in a specified group by group index.
        /// </summary>
        /// <param name="groupIndex">The index of the group.</param>
        /// <returns>An array of tuples containing parameter names and their indices.</returns>
        /// <exception cref="EmptyParameterGroupException">
        /// Thrown if the specified group does not contain any parameters.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the specified group does not exist.
        /// </exception>
        public (string, int)[] ListGroupParameters(int groupIndex)
        {
            if (_groupValuesByIndex.TryGetValue(groupIndex, out (string, int)[]? values))
            {
                if (values.Length == 0 || values == null)
                {
                    throw new EmptyParameterGroupException($"The group at index \"{groupIndex}\" does not contain any parameters.");
                }
                return values;
            }
            else { throw new KeyNotFoundException($"The group at index \"{groupIndex}\" doesn't exist."); }
        }

        /// <summary>
        /// Lists all group names in the collection.
        /// </summary>
        /// <returns>An array of group names.</returns>
        public string[] ListGroups()
        {
            return _groupValuesByName.Keys.ToArray();
        }
    }

}
