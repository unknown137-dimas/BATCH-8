public class Fish : Animal

{
	string color = "";
	string type = "";
	public Fish(string name, int age, string color, string type): base(name, age)

	{
		this.color = color;
		this.type = type;
	}

	public void Swim() => Console.WriteLine($"{_name} is Swimming... ><  (°>");

	public string SayType() => $"I am a {this.type} fish with body color of {this.color}.";

	public string RepeatTheWords(params string[] words)

	{
		string repeatedWords = $"{_name} says: ";
		foreach (string word in words)

		{
			repeatedWords += word + " ";
		}
		return repeatedWords;
	}

    public override string Speak() => "Blub... Blub...";
}