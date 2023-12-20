
class Program

{
	static void Main()

	{
		Fish bob = new("Bobby", 1, "Red", "Koi");
		bob.Swim();
		Console.WriteLine(bob.SayHi());
		Console.WriteLine(bob.SayType());
		Console.WriteLine(bob.RepeatTheWords("I", "Love", "U"));
		Console.WriteLine(bob.Eat("Steak"));
		Console.WriteLine(bob.Speak());
		
		Cat chesire = new("Chesire", 100, "Blue", "Imaginary");
		chesire.Walk();
		Console.WriteLine(chesire.SayHi());
		Console.WriteLine(chesire.SayType());
		Console.WriteLine(chesire.RepeatTheWords("Hello", "there...", "fellas..."));
		Console.WriteLine(chesire.Eat("Fish"));
		Console.WriteLine(chesire.Speak());
	}
}