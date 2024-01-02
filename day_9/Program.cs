class Program

{
	static void Main()
	
	{
		// HashSet
		Console.WriteLine("\nHashSet");
		HashSet<int> hashOne = new();
		hashOne.Add(1);
		hashOne.Add(2);
		hashOne.Add(3);
		Console.Write("HashSet 1 : ");
		foreach (var item in hashOne)
		{
			Console.Write($"{item} ");
		}
		Console.WriteLine();
		
		HashSet<int> hashTwo = new();
		hashTwo.Add(2);
		hashTwo.Add(3);
		hashTwo.Add(4);
		Console.Write("HashSet 2 : ");
		foreach (var item in hashTwo)
		{
			Console.Write($"{item} ");
		}
		Console.WriteLine();
		
		hashOne.UnionWith(hashTwo);
		Console.Write("Result of UnionWith : ");
		foreach (var item in hashOne)
		{
			Console.Write($"{item} ");
		}
		Console.WriteLine();
		
		// Queue
		Console.WriteLine("\nQueue");
		Queue<int> myQueue = new();
		myQueue.Enqueue(1);
		myQueue.Enqueue(2);
		myQueue.Enqueue(3);
		
		Console.Write("Queue : ");
		foreach (var item in myQueue)
		{
			Console.Write($"{item} ");
		}
		Console.WriteLine();
		
		Console.WriteLine($"Dequeue : {myQueue.Dequeue()}");
		Console.WriteLine($"Peek : {myQueue.Peek()}");
		
		// Stack
		Console.WriteLine("\nStack");
		Stack<int> myStack = new();
		myStack.Push(1);
		myStack.Push(2);
		myStack.Push(3);
		
		Console.Write("Stack : ");
		foreach (var item in myStack)
		{
			Console.Write($"{item} ");
		}
		Console.WriteLine();
		
		Console.WriteLine($"Pop : {myStack.Pop()}");
		Console.WriteLine($"Peek : {myStack.Peek()}");
		
		// Dictionary
		Console.WriteLine("\nDictionary");
		Dictionary<int, string> myDictionary = new();
		myDictionary.Add(0, "Dimas");
		myDictionary.Add(1, "Fitrio");
		myDictionary.Add(2, "Kurniawan");
		
		foreach(var item in myDictionary)
		{
			Console.WriteLine($"{item.Key} : {item.Value}");
		}
		
		Console.WriteLine($"Contains Key '3'? : {myDictionary.ContainsKey(3)}");
	}
}