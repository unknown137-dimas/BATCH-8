class Program
{
	static void Main()
	{
		// StreamWriter
		string path = @"output.txt";
		FileStreamOptions options = new()
		{
			Mode = FileMode.OpenOrCreate,
			Access = FileAccess.Write,
			Share = FileShare.None
		};
		// using(FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
		// {
		// }
		using(StreamWriter writer = new(path, options))
		{
			writer.WriteLine("Annyeonghaseyo");
			Console.ReadKey();
		}
		
		// StreamReader
		// using(FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Read))
		// {
		// }
		using(StreamReader reader = new(path))
		{
			Console.WriteLine(reader.ReadToEnd());
		}
	}
}