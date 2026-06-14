
namespace SHARP3D.Parameter.DataEntity.Clean
{
    public class C3dParameter
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
        public int[] Dimensions = new int[] { };
        public Array Data = new int[] { };
        public bool Locked = false;
    
        public C3dParameter(string name, string description, Array data)
        {
            Name = name;
            Description = description;
            Data = data;
            List<int> tempDimensions = new List<int> { };
            for (int i = 0; i < Data.Rank; i++)
            {
                tempDimensions.Add(Data.GetLength(i));
            }
            Dimensions = tempDimensions.ToArray();
        }
    }
}
