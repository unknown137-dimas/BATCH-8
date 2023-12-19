
using Animal;

class Program

{
	public static void Main()
	
	{
		Fish bob = new Fish("Bobby", 1, "Red", "Koi");
		Console.WriteLine(bob.SayHi());
		bob.Swim();
	}
}