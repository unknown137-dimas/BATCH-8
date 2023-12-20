public class Fish : Animal

{
	public readonly string color = "";
	public readonly string type = "";
	public Fish(string name, int age, string color, string type): base(name, age)

	{
		this.color = color;
		this.type = type;
	}

	public void Swim() => Console.WriteLine($"{_name} is Swimming... ><  (°>");

	public string SayType() => $"I am a {this.type} fish with body color of {this.color}.";

    public override string Speak() => "Blub... Blub...";
}