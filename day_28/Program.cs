class Program
{
	static void Main()
	{
		// GC & Finalizer
		var obj = new MyClass();
		// GC.Collect();
		
		// Dispose
		// obj.Dispose();
		
		// using
		using(obj)
		{
			Console.WriteLine("Do something inside 'using' keyword");
		}
	}
}

class MyClass : IDisposable

{
	private bool disposedValue;

	public MyClass()
	
	{
		Console.WriteLine("Hey There");
	}
	
	~MyClass()
	
	{
		Console.WriteLine("Adiosss");
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
				Console.WriteLine("Adiosss from Dispose");
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			disposedValue = true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~MyClass()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}