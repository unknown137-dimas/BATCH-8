public class Animal

{
	protected readonly string _name = "";
	protected readonly int _age = 0;

	public Animal(string name, int age) 
	{
		_name = name;
		_age = age;
	}
	public Animal(){}

	public string SayHi() => $"Hi, my name is {_name} and i'm {_age} years old.";
	public string Eat(string food) => $"Eating {food}, nyam nyam nyam";
	public string RepeatTheWords(params string[] words)

	{
		string repeatedWords = $"{_name} says: ";
		foreach (string word in words)

		{
			repeatedWords += word + " ";
		}
		return repeatedWords;
	}
	
	public virtual string Speak() => "Animal Speak";
}