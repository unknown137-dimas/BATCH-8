abstract class Animal
{
	protected int age = 0;
	protected string name = "";
	public Animal(int age, string name)
	{
		this.age = age;
		this.name = name;
	}
	public abstract void MakeSound();
	public virtual void Eat(string food) => Console.WriteLine($"{name} is eating {food} nyam... nyam... nyam...");
	public virtual void SayHi() => Console.WriteLine($"Hi, my name is {name} and i am {age} years old :)");
}