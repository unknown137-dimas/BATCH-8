public class Cat : Animal

{
	string color = "";
	string type = "";
	public Cat(string name, int age, string color, string type): base(name, age)

	{
		this.color = color;
		this.type = type;
	}

	public void Walk() => Console.WriteLine($"{name} is Walking... (^._.^)/");

	public string SayType() => $"I am a {this.type} cat with body color of {this.color}.";

    public override string Speak() => "Meow...";
}