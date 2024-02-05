namespace DecoratorExample;

public class Sausage : NoodleDecorator
{
    public Sausage(INoodle noodle) : base(noodle)
    {
    }

    public override int Cost() => base.Cost() + 1500;
    public override string ToString() => $"{base.ToString()} + Sausage";
}
