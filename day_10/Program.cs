class Program
{
	static void Main()
	{
		string a = String.Empty;
		int iter = 10000;
		for(int i = 0; i < iter; i++)
		{
			a += "Hello";
			a += " World";
		}
	}
}