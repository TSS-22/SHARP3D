def traverse_nested_list(nested_list, indices=None, dim=0):
    if indices is None:
        indices = [0] * len(nested_list)

    if dim == len(indices) - 1:
        # Base case: Last dimension, iterate and print
        for i in range(len(nested_list)):
            indices[dim] = i
            print(f"Element at {indices}: {nested_list[i]}")
    else:
        # Recursive case: Traverse the current dimension
        for i in range(len(nested_list)):
            indices[dim] = i
            traverse_nested_list(nested_list[i], indices, dim + 1)

# Example usage:
dimensions = [3, 4, 2]
nested_list = [[[i + j*3 + k*12 for i in range(dimensions[2])] for j in range(dimensions[1])] for k in range(dimensions[0])]
traverse_nested_list(nested_list, [0] * len(dimensions))
