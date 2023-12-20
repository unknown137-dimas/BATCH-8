
class Program

{
	public static void Main()

	{
		Fish bob = new("Bobby", 1, "Red", "Koi");
		bob.Swim();
		Console.WriteLine(bob.SayHi());
		Console.WriteLine(bob.SayType());
		Console.WriteLine(bob.RepeatTheWords("I", "Love", "U"));
		Console.WriteLine(bob.Eat("Steak"));
	}
}