namespace DecoratorExample;

public class Cheese : NoodleDecorator
{
    public Cheese(INoodle noodle) : base(noodle)
    {
    }

    public override int Cost() => base.Cost() + 2000;
    public override string ToString() => $"{base.ToString()} + Cheese";
}
