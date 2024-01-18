using BenchmarkDotNet.Running;


class Program

{
	static void Main()
	{
		// Benchmark
		BenchmarkRunner.Run<CollectionBenchmark>();
	}
}