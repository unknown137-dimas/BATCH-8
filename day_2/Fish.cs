class Fish : Animal

{
	public Fish(string name, int age, string color, string type)

	{
		this.name = name;
		this.age = age;
		this.color = color;
		this.type = type;
	}

	public void Swim() => Console.WriteLine($"{this.name} is Swimming... ><  (°>");

	public string SayType() => $"I am a {this.type} fish with body color of {this.color}.";

	public string RepeatTheWords(params string[] words)

	{
		string repeatedWords = $"{this.name} says: ";
		foreach (string word in words)

		{
			repeatedWords += word + " ";
		}
		return repeatedWords;
	}
}