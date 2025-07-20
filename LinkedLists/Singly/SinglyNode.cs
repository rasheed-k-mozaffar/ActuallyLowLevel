using System;

namespace LowLevelLinkedLists.Singly;

public unsafe struct SinglyNode
{
    /// <summary>
    /// The data stored in the node
    /// </summary>
    public int Data;

    /// <summary>
    /// A pointer to the next node in the list
    /// </summary>
    public SinglyNode* Next;
}
