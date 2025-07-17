using System;
using System.Runtime.InteropServices;

namespace LowLevelLinkedLists;

public unsafe class SinglyLinkedList : IDisposable
{
    private Node* _head = null;

    public void Add(int data)
    {
        Node* newNode = (Node*)NativeMemory.AllocZeroed((nuint)sizeof(Node));
        newNode->Data = data;
        newNode->Next = null;

        if (_head is null)
            _head = newNode;
        else
        {
            Node* current = _head;
            while (current->Next is not null)
            {
                current = current->Next;
            }
        }
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
