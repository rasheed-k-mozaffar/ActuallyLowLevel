using System;
using System.IO.Compression;
using System.Runtime.InteropServices;
using LowLevelLinkedLists.Doubly;

namespace LowLevelLinkedLists;

public unsafe class DoublyLinkedList : IDisposable
{
    private bool _disposed = false;

    // Pointer to the first node in the list
    private DoublyNode* _head = null;

    // Pointer to the last node in the list
    private DoublyNode* _tail = null;


    /// <summary>
    /// Adds a new item to the list at the end
    /// </summary>
    /// <param name="data"></param>
    /// <param name="data"></param>
    public void Add(int data)
    {
        // Allocate memory for the new node.
        DoublyNode* newNode = (DoublyNode*)NativeMemory.AllocZeroed((nuint)sizeof(DoublyNode));
        newNode->Data = data;
        // Ensure new node's next doesn't point to anything
        newNode->Previous = null;
        newNode->Next = null;

        // If the list is empty, head and tail will both point to the same node.
        if (_head is null)
        {
            _head = newNode; // Set head to new node
            _tail = newNode; // Set tail to new node
            return;
        }

        // Set current tail's Next to the new node.
        _tail->Next = newNode;
        // Link the new node to the previous node (Ex tail)
        newNode->Previous = _tail;
        // Update the tail to point to the new node
        _tail = newNode;
    }

    public void Add(int atIndex, int data)
    {

    }




    public void Display()
    {
        Console.Write("Head <-> ");
        DoublyNode* current = _head;
        while (current != null)
        {
            Console.Write($"[Data: {current->Data} | Addr: {(long)current:X}] <-> ");
            current = current->Next;
        }
        Console.WriteLine("NULL");
    }


    public void DisplayReversed()
    {
        Console.WriteLine("Tail <-> ");
        DoublyNode* current = _tail;
        while (current is not null)
        {
            Console.Write($"[Data: {current->Data} | Addr: {(long)current:X}] <-> ");
            current = current->Previous;
        }
        Console.WriteLine("NULL");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
    }

    ~DoublyLinkedList()
    {
        Dispose(false);
    }
}
