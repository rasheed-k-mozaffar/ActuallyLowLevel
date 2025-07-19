using System;

namespace LowLevelLinkedLists;

public unsafe struct Node
{
    /// <summary>
    /// The data stored in the node
    /// </summary>
    public int Data;

    /// <summary>
    /// A pointer to the next node in the list
    /// </summary>
    public Node* Next;
}
