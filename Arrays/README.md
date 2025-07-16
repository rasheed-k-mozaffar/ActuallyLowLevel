# Arrays - Actually Low Level 
This document explains how arrays work on the more low-level side without the abstractions of high-level programming languages.

## What is an Array?
An array is simply defined as follows:
> A fixed-size collection of items that are stored in contiguous memory locations that are of the same data type. Arrays provides constant-time access to its elements through what's known as an index, which is an integer value representing an offset from the start of the array that points to the elements in the sequence. Arrays are mostly always zero-indexed, meaning the first item is stored at index 0, and not 1.
This simple gives us enough details to understand what an array is, which we can break down into these following points:
- Arrays have a fixed size (in most programming languages)
- Arrays store data of the same data type (again, in most languages)
- Arrays store their items in sequential, contiguous memory addresses
- Arrays elements are accessed through an index, which is an integer label for the element, indicating the element's offset from the start of the array.

## Why arrays are fast when accessing an item?
As mentioned above, array elements are accessed through the index of the desired item. To read an item from an array at any position say x, all we need to know is the starting index of the array's base address is, the size of the element, and we can use that to calculcate the position of the element x we're looking for using this simple formula

`ELEMENT_MEMORY_LOCATION = BASE_ADDRESS + (INDEX * ELEMENT_SIZE)`

Let's say we have this array of integers: `[1, 2, 3, 4, 5]`.
When we create an array, we store a pointer to the base address which is the starting point of the array, and since this array is an array of integers (assuming 32-bit integers or int 32), the element size is 4 bytes (32 bits). Let's also assume that the base address gave us this arbitrary pointer `2000`, to get the element at index 3, which in our case is 4 (remember arrays are zero-indexed), then we can plug all the variabels into the formula:

`ELEMENT_MEMORY_LOCATION = 2000 + (3 * 4)`

This can read like this: The memory address of the element x, is equal to the starting base address of the array, with an offset of 12, essentially we're taking strides overy that sequential block of memory that's allocated for our array, until we reach the location of the element we're looking for, which is `4`, at memory address `2012`.

---

## 🛠️ How does `LowLevelArray` model a low-level array?

The `LowLevelArray` in this project is my way of showing how arrays work under the hood, without all the safety nets and magic of high-level languages. Here’s how it works, in a way that’s easy to understand no matter what language you use:

## Grabbing a reference to the base addresss
When the array is created, there are some checks inside the constructors of the array, which will be explained later, we allocate enough memory to store all the elements, and we grab a reference to the base address throuhg this code:
```csharp
var totalBytes = length * sizeof(T);
_baseAddress = (T*)NativeMemory.AllocZeroed((nuint)totalBytes);
```
This is still using some language abstractions, but that's fine, the goal is to have a good understanding of what actually goes on when dealing with arrays. Now that we have the base address, through the indexer we have on the array class, which is just a way of providing us the ability to read and write to our array with a nice syntax, that is the square brackets we're used when dealing with arrays in almost all programming languages.

## Reading/Writing to the array
```csharp
get
{
    // Bounds check
    if (index < 0 || index >= _length)
        throw new IndexOutOfRangeException($"The index {index} is outside the bounds of the array");

    return *(_baseAddress + index);
}
set
{
    // Bounds check
    if (index < 0 || index >= _length)
        throw new IndexOutOfRangeException($"The index {index} is outside the bounds of the array");

    *(_baseAddress + index) = value;
}
```

This code is what's allowing us to read and write to our array, both sections start with a bounds, and this is important when dealing with arrays, because accessing an item outside the bounds of the array could cause the application to crash and have other unwanted side effects. To validate that the index is within the bounds, it has to satisfy this condition: 
`0 <= index < array length`

After the bounds check, we use the aforementioned formula to read/write to that memory location. What we're ultimately doing, is that we're accessing the memory location by calculcating its address `*(_baseAddress + index)`, this internally calculcates the size of the item based on the type and takes the necessary strides to get to the required position, and then we dereference the pointer to read the value at the locationn in the `get`, and for the set, we write to that position.

## Copy Constructor
The `LowLevelArray<T>` class has three constructors overloads, one of which takes a source array, which it copies the values from to construct a new copy of the source array. 
```csharp
public LowLevelArray(LowLevelArray<T> source)
{
    var length = source.Length;
    if (length < 0)
        throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative");

    _length = length;
    var totalBytes = length * sizeof(T);
    _baseAddress = (T*)NativeMemory.AllocZeroed((nuint)totalBytes);

    // Copy the values from the source array 
    for (int i = 0; i < length; i++)
    {
        *(_baseAddress + i) = *(source._baseAddress + i);
    }
}
```

Here's a step-by-step breakdown of what's happening:
1. The length is checked for negatvitiy, so that we guard against negative numbers.
1. The `_length` field is assigned so that we have access to it through the `Length` property.
1. Total number of bytes to allocate is calculated by multiplying the length of the array, with the size of each element.
1. Total bytes are allocated in the memory and zeroed (Bytes are set to 0 to avoid dirty memory from previous uses) and a reference to the base address is assigned to `_baseAddress`.
1. A loop is used to iterate through the source array and copy its values to the new array.
1. `*(_baseAddress + i) = *(source._baseAddress + i);` sets the value at each memory address of the array to the corresponding value from the source array. This is repeated until the code has iterated over the entirity of the source array.