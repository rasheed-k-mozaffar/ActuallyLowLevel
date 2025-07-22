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
        // Validate the index is not negative.
        if (atIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(atIndex), "Index cannot be negative.");

        // Allocate memory for the new node.
        DoublyNode* newNode = (DoublyNode*)NativeMemory.AllocZeroed((nuint)sizeof(DoublyNode));
        newNode->Data = data;
        // Ensure new node's next doesn't point to anything
        newNode->Previous = null;
        newNode->Next = null;

        // Handle inserting at first
        if (atIndex == 0)
        {
            // If the list is empty, the new node is both head and tail.
            if (_head is null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else // Otherwise, it becomes the new head.
            {
                newNode->Next = _head;
                _head->Previous = newNode;
                _head = newNode;
            }
            return;
        }

        // Traverse the list till we reach the node at the given index
        DoublyNode* current = _head;
        for (int i = 0; i < atIndex; i++)
        {
            // If current becomes null before find the index, the index is outside the bounds of the list.
            if (current is null)
            {
                // Clean up the memory allocated for the new node
                NativeMemory.Free(newNode);
                throw new ArgumentOutOfRangeException(nameof(atIndex), "Index is outside the bounds of the list.");
            }
            current = current->Next;
        }

        // If we landed at null then we reached to the position after the tail, so we append at the item at the end
        if (current is null)
        {
            _tail->Next = newNode;
            newNode->Previous = _tail;
            _tail = newNode; // tail becomes the new node
        }
        else
        {
            // 'current' is the node that will come AFTER the new node
            // 'previousNode' is the node that will come BEFORE the new node
            DoublyNode* previousNode = current->Previous;

            // Connect the new node to the node after it
            newNode->Next = current;
            // Connect the new node to the node before it
            newNode->Previous = previousNode;

            // Connect the node before new one and the one after it
            previousNode->Next = newNode;
            current->Previous = newNode;
        }
    }

    /// <summary>
    /// Removes an item at a given index.
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Remove(int index)
    {
        if (index < 0 || _head is null)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of bounds.");

        // Remove the head node
        if (index == 0)
        {
            // Store the head in a temp node to be deleted
            DoublyNode* nodeToRemove = _head;
            // Move the head to the next node
            _head = _head->Next;

            // Added check: If the list is now empty, the tail must also be null
            if (_head is null)
                _tail = null;
            else
                // The new head's previous pointer must be updated to null
                _head->Previous = null;

            // Free the allocated memory for the old head node
            NativeMemory.Free(nodeToRemove);
            return;
        }

        DoublyNode* current = _head;
        // This traversal logic is correct for finding the node at the given index.
        for (int i = 0; i < index; i++)
        {
            current = current->Next;
            // If current becomes null then the index is outside the list's bounds
            if (current is null)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is outside the bounds of the list.");
        }

        // 'current' is now the node we need to remove.

        // Remove at the tail
        if (current == _tail) // Using a direct check against _tail is slightly cleaner
        {
            // The node to remove is the tail
            _tail = current->Previous;
            // The new tail must not point to anything next
            _tail->Next = null;
            // Free the memory for the old tail
            NativeMemory.Free(current);
        }
        else // Remove from the middle
        {
            // Store the node to remove
            DoublyNode* nodeToRemove = current;
            // Get the links of the node to delete
            DoublyNode* previous = nodeToRemove->Previous;
            DoublyNode* next = nodeToRemove->Next;

            previous->Next = next;
            next->Previous = previous;
            NativeMemory.Free(nodeToRemove);
        }
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
