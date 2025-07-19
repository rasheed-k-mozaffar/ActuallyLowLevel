using System.Runtime.InteropServices;

namespace LowLevelLinkedLists;

public unsafe class SinglyLinkedList : IDisposable
{
    private bool _disposed = false;

    // Pointer to the first node in the list
    private Node* _head = null;

    /// <summary>
    /// Adds a new item to the list at the end
    /// </summary>
    /// <param name="data"></param>
    public void Add(int data)
    {
        // Allocate zeroed memory for the new node
        Node* newNode = (Node*)NativeMemory.AllocZeroed((nuint)sizeof(Node));

        // Initialize the new node with the data received.
        newNode->Data = data;
        // Ensure the node is going to be last
        newNode->Next = null;

        // If the list is empty, the new node becomes the head node
        if (_head is null)
        {
            _head = newNode;
            return;
        }

        // If the list is not empty, traverse till the end of the list
        Node* current = _head;
        while (current->Next is not null)
        {
            current = current->Next;
        }

        // Connect the lat node to the newly created node
        current->Next = newNode;
    }

    public void Add(int atIndex, int data)
    {
        // Validate the index is not negative
        if (atIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(atIndex), "Index cannot be negative.");

        // Allocate memeory for the new node and initialize it
        Node* newNode = (Node*)NativeMemory.AllocZeroed((nuint)sizeof(Node));
        newNode->Data = data;

        // Handle inserting at first
        if (atIndex == 0)
        {
            newNode->Next = _head; // Point the new node's next to the current head
            _head = newNode; // Set the new head to the new node
            return;
        }

        // Keep a pointer to the current node
        Node* current = _head;

        // Traverse to the item at index - 1 from where we want to add
        for (int i = 0; i < atIndex - 1; i++)
        {
            // If current becomes null then index it outside the list's bounds
            if (current is null)
            {
                NativeMemory.Free(newNode);
                throw new ArgumentOutOfRangeException(nameof(atIndex), "Index is outside the bounds of the list.");
            }
            current = current->Next; // Move to the next item
        }

        // Validate the item before we want to insert is not null, otherwise the index is out of the list's bounds.
        if (current is null)
        {
            NativeMemory.Free(newNode);
            throw new ArgumentOutOfRangeException(nameof(atIndex), "Index is outside the bounds of the list.");
        }

        newNode->Next = current->Next;
        current->Next = newNode;
    }

    // Displays the list's contents
    public void Display()
    {
        Console.Write("Head -> ");
        Node* current = _head;
        while (current != null)
        {
            Console.Write($"[Data: {current->Data} | Addr: {(long)current:X}] -> ");
            current = current->Next;
        }
        Console.WriteLine("NULL");
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

        // Free the memory allocated for the list's nodes.
        if(_head is not null)
        {
            Node* current = _head;
            
            // Traverse the list and free each node.
            while(current is not null)
            {
                Node* next = current->Next;
                NativeMemory.Free(current);
                current = next;
            }
        }

        // Nullify the head pointer to avoid future misuse.
        _head = null;

        // Mark the object as disposed.
        _disposed = true;
    }

    ~SinglyLinkedList()
    {
        // This is called by the Garbage Collector if the user forgets to call Dispose().
        // We pass 'false' because we should not touch managed objects here.
        Dispose(false);
    }
}
