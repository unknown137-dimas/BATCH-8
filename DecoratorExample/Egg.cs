namespace DecoratorExample;

public class Egg : NoodleDecorator
{
	public Egg(INoodle noodle) : base(noodle)
	{
	}

	public override int Cost()
	{
		return base.Cost() + 1000;
	}

    public override string ToString()
    {
        return $"{base.ToString()} + Egg";
    }
}
