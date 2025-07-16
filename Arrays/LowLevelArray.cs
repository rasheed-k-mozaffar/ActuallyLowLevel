using System.Runtime.InteropServices;

namespace LowLevelArrays;

public unsafe class LowLevelArray<T> : IDisposable where T : unmanaged
{
    private T* _baseAddress;
    private readonly int _length;
    private bool _disposed = false;

    /// <summary>
    /// Gets or Sets the value at a specific index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public T this[int index]
    {
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
    }

    /// <summary>
    /// Gets the length of the array. Returns the value passed when creating the arrray.
    /// </summary>
    public int Length { get => _length; }

    /// <summary>
    /// Creates a new instance of the low level array with the specified length.
    /// </summary>
    /// <param name="length"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public LowLevelArray(int length)
    {
        // Guard against negative array size
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative");

        // Set the length
        _length = length;

        // Allocate a contiguous block of zeroed memory for the array on the unmanaged heap.
        // Zeroed allocation will clear any dirty memory on that block to avoid unpredicable behavior.
        var totalBytes = length * sizeof(T);
        _baseAddress = (T*)NativeMemory.AllocZeroed((nuint)totalBytes);
    }

    /// <summary>
    /// Creates a new instance of the low level array with the values from the source array copied to the new array.
    /// </summary>
    /// <param name="source"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    public void Dispose()
    {
        Dispose(true);
        // Tell the GC not to run the finalizer, since we've already cleaned up.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Check if we've already disposed.
        if (_disposed)
            return;

        if (_baseAddress is not null)
        {
            // Free the unmanaged memory block allocated for the array
            Marshal.FreeHGlobal((IntPtr)_baseAddress);
            // Nullify the pointer to avoid future misuse.
            _baseAddress = null;
        }

        _disposed = true;
    }

    ~LowLevelArray()
    {
        // This is called by the Garbage Collector if the user forgets to call Dispose().
        // We pass 'false' because we should not touch managed objects here.
        Dispose(false);
    }
}
