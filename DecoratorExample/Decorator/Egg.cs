namespace DecoratorExample;

public class Egg : NoodleDecorator
{
    public Egg(INoodle noodle) : base(noodle)
    {
    }

    public override int Cost() => base.Cost() + 1000;
    public override string ToString() => $"{base.ToString()} + Egg";
}
