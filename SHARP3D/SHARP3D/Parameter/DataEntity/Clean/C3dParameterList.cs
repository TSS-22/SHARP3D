namespace SHARP3D.Parameter.DataEntity.Clean
{
    public class C3dParameterList
    {
        private List<C3dParameter> _parameterList = new List<C3dParameter>();

        public C3dParameter this[int index]
        {
            get
            {
                return _parameterList[index];
            }
            set
            {
                // Check for duplicate parameter label
                if (_parameterList.Any(p => p.Name == value.Name.ToUpper()))
                {
                    throw new ArgumentException($"A parameter with the name '{value.Name}' already exists in this group."); // I don't know how to give the group name from here.
                }
                // We check in the C3dParameter setter if the parameter name is reserved or not. So we don't need to check it here.
                _parameterList[index] = value;
            }
        }
    }
}
