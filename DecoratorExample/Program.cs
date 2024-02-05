#define DECORATOR

using DecoratorExample;

// Inheritance Approach
#region INHERITANCE
class CheeseNoodle : Noodle
{
    public override int Cost() => base.Cost() + 2000;
    public override string ToString() => $"{base.ToString()} + Cheese";
}

class SausageNoodle : Noodle
{
    public override int Cost() => base.Cost() + 1500;
    public override string ToString() => $"{base.ToString()} + Sausage";
}

class EggNoodle : Noodle
{
    public override int Cost() => base.Cost() + 1000;
    public override string ToString() => $"{base.ToString()} + Egg";
}

class CheeseSausageNoodle : Noodle
{
    public override int Cost() => base.Cost() + 3500;
    public override string ToString() => $"{base.ToString()} + Sausage + Cheese";
}
#endregion

class Program
{
    static void Main()
    {
        // Inheritance Approach
        #if INHERITANCE
        var noodleOrder = new CheeseNoodle();
        #endif

        // Decorator Approach
        #if DECORATOR
        var noodleOrder = new Cheese(new Noodle());
        #endif

        Console.WriteLine($"Noodle Order: {noodleOrder}");
        Console.WriteLine($"Total Price: {noodleOrder.Cost()}");
    }
}