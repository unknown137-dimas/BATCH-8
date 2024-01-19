class Program
{
	public static SemaphoreSlim semaphore = new SemaphoreSlim(3);
	static async Task Main()
	{
		// Async & Await
		Console.WriteLine("Start");
		await DontNeedToWaitForMe();
		Console.WriteLine("Finish");
		// await MyMethod();
		// Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
		
		// Cancel Token
		// CancellationTokenSource source = new();
		// CancellationToken token = source.Token;
		// Task task = CancelMe(token);
		// if(Console.ReadKey().KeyChar == 'c')
		// {
		// 	source.Cancel();
		// }
		// try
		// {
		// 	await task;
		// }
		// catch(Exception)
		// {
		// 	Console.WriteLine("Task Cancelled");
		// }
		// finally
		// {
		// 	Console.WriteLine("Task Completed");
		// }
		
		// Tread Queue
		Task[] tasks = new Task[10];
		for (int i = 1; i <= 10; i++)
		{
			int taskId = i;
			tasks[i - 1] = Task.Run( () =>  DoingSomethingImportant(taskId, i));
		}

		await Task.WhenAll(tasks);
		Console.WriteLine("All task finished");
	}
	
	static async Task<int> DontNeedToWaitForMe()
	{
		await Task.Delay(5000);
		return 800;
	}
	
	static int WaitForMe()
	{
		Thread.Sleep(5000);
		return 800;
	}
	
	static async Task CancelMe(CancellationToken token)
	{
		for(int i = 0; i < 10; i++)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}
			Console.WriteLine($"Progress {i*10}%");
			await Task.Delay(1000, token);
		}
	}
	
	static Task MyMethod()
	{
		// Console.WriteLine($"Task thread: {Thread.CurrentThread.ManagedThreadId}");
		return Task.Delay(5000);
	}
	
	static async Task MyMethodAsync()
	{
		// Console.WriteLine($"Task thread: {Thread.CurrentThread.ManagedThreadId}");
		await Task.Delay(5000);
	}
	
	static async void DoingSomethingImportant(int taskId, int index)
	{
		Console.WriteLine($"Task {taskId} Wants to Enter");
		try
		{   
			semaphore.Wait();
			Console.WriteLine("Success: Task " + taskId + " is Doing its work");
			await Task.Delay(7000);
			Console.WriteLine($"Task {taskId} Exit.");
		}
		finally
		{
			semaphore.Release();
		}
	}
}