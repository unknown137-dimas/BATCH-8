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
	
	
	public void Swim()
	
	{
		Console.WriteLine($"{this.name} is Swimming");
	}
	
	public string SayHi()
	
	{
		return $"Hi, my name is {name} and  i'm {age} years old. I am a {type} with body color of {color}";
	}
}