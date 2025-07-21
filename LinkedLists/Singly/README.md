# Linked Lists - Actually Low Level 
This document explains how linked lists work on the more low-level side without the abstractions of high-level programming languages.

## What is a Linked List?
> Unlike arrays, linked list is a **non-contiguous** data structure that's also not fixed in size. Elements are stored in separate blocks of memory mostly referred to as `Nodes`. Each node contains the data itself, and a `Pointer` storing the memory address of the next node in the sequence.

This definition gives us the key differences between arrays and linked lists, which we can break down into these following points:
- Linked lists are **not fixed in size** (they can grow and shrink dynamically)
- Linked lists store data in **separate memory locations** (non-contiguous)
- Each element in a linked list is called a **Node**
- Each node contains **two things**: the actual data and a pointer to the next node
- The last node points to `null` to indicate the end of the list

## Why are linked lists different from arrays?
The main difference comes down to how memory is organized:

**Arrays**: All elements are stored in one continuous block of memory
```
Memory: [Element1][Element2][Element3][Element4][Element5]
        ↑
    Base Address
```

**Linked Lists**: Each element is stored in a separate memory location, connected by pointers
```
Memory: [Node1] → [Node2] → [Node3] → [Node4] → [Node5] → NULL
         ↑        ↑         ↑         ↑         ↑
      Address1  Address2  Address3  Address4  Address5
```

## Why linked lists are slower for random access?
Unlike arrays where you can jump directly to any element using the formula `BASE_ADDRESS + (INDEX * ELEMENT_SIZE)`, linked lists require you to **traverse** from the beginning of the list to reach a specific element.

To get to the 4th element in a linked list, you have to:
1. Start at the head (first node)
2. Follow the pointer to the 2nd node
3. Follow the pointer to the 3rd node  
4. Follow the pointer to the 4th node

This is why accessing elements in a linked list is **O(n)** (linear time) instead of **O(1)** (constant time) like arrays.

---

## 🛠️ How does `SinglyLinkedList` model a low-level linked list?

The `SinglyLinkedList` in this project demonstrates how linked lists work under the hood, showing the manual memory management and pointer manipulation that happens behind the scenes.

## The Node Structure 🔗
Each node in a linked list is a simple structure that contains two things:
```csharp
public unsafe struct Node
{
    public int Data;        // The actual data we want to store
    public Node* Next;      // A pointer to the next node in the list
}
```

This is the fundamental building block of any linked list. The `Data` field holds our value, and the `Next` pointer tells us where to find the next node in the sequence. If `Next` is `null`, we've reached the end of the list.

## Creating and Adding Nodes ➕
When we add a new item to the list, here's what happens at a low level:

```csharp
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

    // Connect the last node to the newly created node
    current->Next = newNode;
}
```

Here's a step-by-step breakdown of what's happening:
1. **Memory Allocation**: We ask the system for a chunk of memory big enough to hold one `Node` structure
2. **Node Initialization**: We set the node's data and make sure it points to `null` (since it will be the last node)
3. **Empty List Check**: If this is the first node, it becomes the head of the list
4. **Traversal**: If the list already has nodes, we walk through the list by following the `Next` pointers until we reach the last node
5. **Connection**: We connect the last node to our new node by setting its `Next` pointer

## Inserting at a Specific Position 📍
The more complex operation is inserting a node at a specific position:

```csharp
public void Add(int atIndex, int data)
{
    // Validate the index is not negative
    if (atIndex < 0)
        throw new ArgumentOutOfRangeException(nameof(atIndex), "Index cannot be negative.");

    // Allocate memory for the new node and initialize it
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
        // If current becomes null then index is outside the list's bounds
        if (current is null)
        {
            NativeMemory.Free(newNode);
            throw new ArgumentOutOfRangeException(nameof(atIndex), "Index is outside the bounds of the list.");
        }
        current = current->Next; // Move to the next item
    }

    // Validate the item before we want to insert is not null
    if (current is null)
    {
        NativeMemory.Free(newNode);
        throw new ArgumentOutOfRangeException(nameof(atIndex), "Index is outside the bounds of the list.");
    }

    newNode->Next = current->Next;
    current->Next = newNode;
}
```

This operation is more complex because we need to:
1. **Handle Special Cases**: If inserting at position 0, we need to update the head pointer
2. **Traverse to the Right Position**: We walk through the list to find the node that comes before our insertion point
3. **Update Pointers**: We carefully update the pointers to maintain the list's structure
4. **Error Handling**: We check for invalid indices and clean up memory if something goes wrong

## Memory Management 🧹
Unlike arrays where all memory is allocated at once, linked lists allocate memory one node at a time. This means:
- **Dynamic Growth**: The list can grow as needed without pre-allocating space
- **Memory Fragmentation**: Nodes can be scattered throughout memory
- **Manual Cleanup**: Each node must be freed individually when the list is destroyed

---

## Removing Nodes from the List 🗑️
Removing nodes from a linked list is more complex than adding them because we need to carefully update the pointers to maintain the list's structure. Here's how the `Remove` method works:

```csharp
public void Remove(int index)
{
    if (index < 0 || _head is null)
        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of");

    // Remove the head node.
    if (index == 0)
    {
        Node* temp = _head;
        _head = _head->Next;
        NativeMemory.Free(temp);
        return;
    }

    // Traverse to the node BEFORE target index.
    Node* current = _head;
    for (int i = 0; i < index - 1; i++)
    {
        if (current->Next is null)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of the bounds of the list.");
        current = current->Next;
    }

    // Ensure the node to be deleted actually exists.
    if (current->Next == null)
        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of bounds of the list.");

    // current now is at node before the target node.
    Node* nodeToDelete = current->Next;
    current->Next = nodeToDelete->Next; // Unlink the node from the list
    NativeMemory.Free(nodeToDelete); // Free the memory the deleted node
}
```

Here's a step-by-step breakdown of what happens when removing a node:

### 1. Input Validation ✅
First, we check if the index is valid and if the list has any nodes to remove:
- If the index is negative, we throw an exception
- If the list is empty (`_head` is null), we throw an exception

### 2. Special Case: Removing the Head Node 🎯
If we're removing the first node (index 0), it's a special case:
```csharp
if (index == 0)
{
    Node* temp = _head;        // Keep a reference to the old head
    _head = _head->Next;       // Make the second node the new head
    NativeMemory.Free(temp);   // Free the memory of the old head
    return;
}
```

This is straightforward because we just need to:
- Save a reference to the current head
- Update the head pointer to point to the second node
- Free the memory of the old head node

### 3. General Case: Removing from Middle/End 🔄
For removing nodes at other positions, we need to:
1. **Traverse to the node BEFORE the target**: We need to find the node that comes before the one we want to delete
2. **Update the pointers**: We need to "skip over" the node we're deleting
3. **Free the memory**: We need to return the deleted node's memory to the system

### 4. The Pointer Dance 💃
The key insight is understanding how we "unlink" a node from the list:

```
Before removal:
[Node A] -> [Node B] -> [Node C] -> [Node D] -> NULL
            ↑
         current (points to node before target)

After removal of Node C:
[Node A] -> [Node B] -> [Node D] -> NULL
            ↑
         current (still points to Node B)
```

The magic happens in these two lines:
```csharp
Node* nodeToDelete = current->Next;           // Get reference to the node we want to delete
current->Next = nodeToDelete->Next;           // Make current point to the node AFTER the deleted one
```

This effectively "jumps over" the node we want to delete, maintaining the list's structure.