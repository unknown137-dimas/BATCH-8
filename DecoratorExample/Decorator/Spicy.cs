namespace DecoratorExample;

public class Spicy : NoodleDecorator
{
    public Spicy(INoodle noodle) : base(noodle)
    {
    }

    public override int Cost() => base.Cost() + 1000;
    public override string ToString() => $"{base.ToString()} + Spicy";
}
