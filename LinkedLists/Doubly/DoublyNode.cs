using System;

namespace LowLevelLinkedLists.Doubly;

public unsafe struct DoublyNode
{
    /// <summary>
    /// The data stored in the node.
    /// </summary>
    public int Data;

    /// <summary>
    /// A pointer to the next node in the list.
    /// </summary>
    public DoublyNode* Next;

    /// <summary>
    /// A pointer to the previous node in the list.
    /// </summary>
    public DoublyNode* Previous;
}
