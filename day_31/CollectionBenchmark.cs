using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class CollectionBenchmark
{
	
	[Params(10, 100, 1000)]
	public int X;
	
	[Benchmark]
	public void DictMethod()
	{
		Dictionary<int, int> dict = new();
		for(int i = 0; i < X; i++)
		
		{
			dict.TryAdd(i, i);
		}
	}
	
	[Benchmark]
	public void ArrayMethod()
	{
		int[] arr = new int[X];
		for(int i = 0; i < X; i++)
		
		{
			arr[i] = i;
		}
	}
}