namespace Animal;
class Fish

{
	public string name = "";
	public int age = 0;
	public string color = "";
	public string type = "";
	
	public Fish(string name, int age, string color, string type)
	
	{
		this.name = name;
		this.age = age;
		this.color = color;
		this.type = type;
	}
	
	public void Swim() => Console.WriteLine($"{this.name} is Swimming... ><  (°>");
	public string SayHi() => $"Hi, my name is {name} and i'm {age} years old.";
	public string SayType() => $"I am a {type} fish with body color of {color}.";
}