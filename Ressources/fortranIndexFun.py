

for (int i = 0; i < vector.Length; i += elementSize)
{
    int remaining = i;
    // Reverse the order of dimensions for Fortran to C# conversion
    for (int d = 0; d < dimensions.Length; d++)
    {
        indices[d] = remaining % dimensions[d];
        remaining /= dimensions[d];
    }
}