class Animal

{
	public string name = "";
	public int age = 0;

	public Animal(string name, int age) {}

	public string SayHi() => $"Hi, my name is {this.name} and i'm {this.age} years old.";
	public string Eat(string food) => $"Eating {food}, nyam nyam nyam";
}