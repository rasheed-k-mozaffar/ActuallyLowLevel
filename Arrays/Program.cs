using LowLevelArrays;

// Create new double array
using var array = new LowLevelArray<double>(5);

// Initialize the array values .
for (int i = 0; i < array.Length; i++)
{
    array[i] = i + 1;
}

// Create a copy of the first array.
using var copy = new LowLevelArray<double>(array);

for (int i = 0; i < copy.Length; i++)
{
    Console.WriteLine(copy[i]);
}

// Create an array with the given values.
using var initialized = new LowLevelArray<int>([1, 2, 3, 4, 5]);

for (int i = 0; i < initialized.Length; i++)
{
    Console.WriteLine(initialized[i]);
}