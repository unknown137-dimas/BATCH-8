public class Animal

{
	protected string _name = "";
	protected int _age = 0;

	public Animal(string name, int age) 
	{
		_name = name;
		_age = age;
	}
	public Animal(){}

	public string SayHi() => $"Hi, my name is {_name} and i'm {_age} years old.";
	public string Eat(string food) => $"Eating {food}, nyam nyam nyam";
}