using System.Numerics;
using System.Runtime.CompilerServices;

public class Car
{
	public int HP { get; private set; }
	public string Name { get; set; }
	public Car(string name, int hp)
	{
		HP = hp;
		Name=name;
	}
	
	// Operator overloading
	public static Car operator +(Car a, Car b) => new Car("", a.HP + b.HP);
}