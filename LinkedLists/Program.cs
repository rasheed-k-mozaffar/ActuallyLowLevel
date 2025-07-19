using LowLevelLinkedLists;

SinglyLinkedList list = new();
list.Add(10);
list.Add(30);
list.Add(40);

list.Add(atIndex: 1, 20);
list.Add(atIndex: 4, 50);

// 10 -> 20 - 30 -> 40 -> 50 -> NULL
list.Display();