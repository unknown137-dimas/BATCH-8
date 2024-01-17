class Program
{
	static void Main()
	{
		// Conditional Compilation
		#if COBA
		#region COBA
		Console.WriteLine("Awalnya sih coba-coba ya... eh keterusan");
		#endregion
		#elif DEBUG
		#region DEBUG
		Console.WriteLine("Daebak");
		#endregion
		#elif RELEASE
		#region RELEASE
		Console.WriteLine("Please release me...");
		#endregion
		#endif
	}
}