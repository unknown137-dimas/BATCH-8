class Program

{
	static void Main()
	
	{
		// Exception Handling
		Console.WriteLine("Exception Handling");
		string input = "1a4"; // try input with not a number
		int result = 0;
		// Bad code (not validate input)
		// Try Catch is bad, try to avoid it as much as possible, because try catch doesn't prevent exception.
		// it's only prevent the program from crashing.
		try
		{
			result = int.Parse(input);
		}
		catch(Exception e)
		{
			Console.WriteLine($"Error occured : {e.Message}");
		}
		finally
		{
			Console.WriteLine($"Try Catch : {result}");
		}
		
		
		// OK Code
		if(int.TryParse(input, out result)) Console.WriteLine($"Validation : {result}"); // Using builtin validation method
		else Console.WriteLine("Validation input failed");
	}
}