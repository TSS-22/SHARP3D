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

        internal float[] GetCalibrationMatrixVector(int idPlate)
        {
            try
            {
                C3dParameter calibrationMatrixParameter = Parameters.GetParameter("force_platform", "cal_matrix");

                float[] calibrationVector = Enumerable.Repeat(1.0f, calibrationMatrixParameter.Dimensions[0]).ToArray();
                if ((Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_2) || (Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_4))
                {
                    for (int i = 0; i < calibrationVector.Length; i++)
                    {
                        try
                        {
                            calibrationVector[i] = calibrationMatrixParameter.Data.GetValue(i, i, idPlate) as float? ?? throw new NullReferenceException();
                        }
                        catch (NullReferenceException)
                        {
                            Console.Error.WriteLine($"WARNING: Force plate id {idPlate} calibration matrix is null at index: [{i},{i}]");
                        }
                    }
                }
                else
                {
                    throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {Required.Forceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                }
                return calibrationVector;
            }
            catch (ArgumentException) { throw; }
            catch (NoCalibrationMatrixForForceplateType) { throw; }
        }

        internal float[,] GetCalibrationMatrix(int idPlate)
        {
            try
            {
                C3dParameter calibrationMatrixParameter = Parameters.GetParameter("force_platform", "cal_matrix");

                float[,] calibrationMatrix = new float[calibrationMatrixParameter.Dimensions[1], calibrationMatrixParameter.Dimensions[0]];
                if ((Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_2) || (Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_4))
                {
                    for (int i = 0; i < calibrationMatrixParameter.Dimensions[1]; i++)
                    {
                        for (int j = 0; j < calibrationMatrixParameter.Dimensions[0]; j++)
                        {
                            calibrationMatrix[i, j] = 1.0f;
                        }
                    }

                    if (Required.Forceplate.Type[idPlate] == ForceplateType.TYPE_2)
                    {
                        for (int col = 0; col < calibrationMatrix.GetLength(0); col++)
                        {
                            for (int row = 0; row < calibrationMatrix.GetLength(1); row++)
                            {
                                try
                                {
                                    calibrationMatrix[row, col] = calibrationMatrixParameter.Data.GetValue(col, row, idPlate) as float? ?? throw new NullReferenceException();
                                }
                                catch (NullReferenceException)
                                {
                                    Console.Error.WriteLine($"WARNING: Force plate id {idPlate} calibration matrix is null at index: [{col},{row}]");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {Required.Forceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                    }
                    return calibrationMatrix;
                }
                else
                {
                    throw new NoCalibrationMatrixForForceplateType($"Force plate id {idPlate} is of type {Required.Forceplate.Type[idPlate]} and therefore don't have calibration matrix.");

                }

            }
            catch (ArgumentException) { throw; }
            catch (NoCalibrationMatrixForForceplateType) { throw; }
        }
    }
}
