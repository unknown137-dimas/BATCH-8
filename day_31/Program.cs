using BenchmarkDotNet.Running;
using System.Threading;
using System.Threading.Tasks;

class Program

{
	static void Main()
	{
		// Benchmark
		// Uncomment to run benchmark
		// BenchmarkRunner.Run<CollectionBenchmark>();
		
		// Concurrency
		Thread thread = new Thread(DoSomething);
		thread.Start();
		thread.Join();
		
		// Task with return value
		Task<int> task = Task.Run(() => CalculateSomething(5));
		task.Wait();
		Console.WriteLine(task.Result);
		
		// Exception handling Thread vs Task
		Thread thread1 = new Thread(DoExceptionHandler);
		thread1.Start();
		thread1.Join();
		
		Task task1 = Task.Run(DoException);
		try
		{
			task1.Wait();
		}
		catch(Exception)
		{
			Console.WriteLine("Exception occured from task");
		}
		
		Console.WriteLine("Finished");
	}
	
	static void DoSomething() => Console.WriteLine("I'm doing some work");
	
	static void DoExceptionHandler()
	{
		try
		{
			DoException();
		}
		catch(Exception)
		{
			Console.WriteLine("Exception occured from handler method");
		}
	}
	static void DoException() => throw new Exception("AAAAAAAAAAAAAAA");

	static int CalculateSomething(int n) => n*n;
}