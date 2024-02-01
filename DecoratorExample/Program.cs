#define DECORATOR
using DecoratorExample;


#region INHERITANCE
class CheeseNoodle() : Noodle
{
	public override int Cost()
	{
		return base.Cost() + 2000;
	}
    public override string ToString()
    {
        return $"{base.ToString()}, Cheese";
    }
}

class SpicyNoodle() : Noodle
{
	public override int Cost()
	{
		return base.Cost() + 1000;
	}
    public override string ToString()
    {
        return $"{base.ToString()}, Spicy";
    }
}

class SpicyCheeseNoodle() : Noodle
{
	public override int Cost()
	{
		return base.Cost() + 3000;
	}
    public override string ToString()
    {
        return $"{base.ToString()}, Spicy, Cheese";
    }
}
#endregion

class Program
{
	static void Main()
	{
		#if INHERITANCE
		var noodleOrder = new SpicyCheeseNoodle();
		#endif
		
		#if DECORATOR
		var noodleOrder = new Spicy(new Spicy(new Spicy(new Egg(new Noodle()))));
		#endif
		
		Console.WriteLine(noodleOrder);
		Console.WriteLine(noodleOrder.Cost());
	}
}