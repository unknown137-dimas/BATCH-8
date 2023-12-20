public class Animal

{
	protected readonly string name = "";
	protected readonly int age = 0;

	public Animal(string name, int age) 
	{
		this.name = name;
		this.age = age;
	}
	public Animal(){}

	public string SayHi() => $"Hi, my name is {this.name} and i'm {this.age} years old.";
	public string Eat(string food) => $"Eating {food}, nyam nyam nyam";
	public string RepeatTheWords(params string[] words)

	{
		string repeatedWords = $"{this.name} says: ";
		foreach (string word in words)

		{
			repeatedWords += word + " ";
		}
		return repeatedWords;
	}
	
	public virtual string Speak() => "Animal Speak";
}