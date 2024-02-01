namespace DecoratorExample;

public class Spicy : NoodleDecorator
{
	public Spicy(INoodle noodle) : base(noodle)
	{
	}

	public override int Cost()
	{
		return base.Cost() + 1000;
	}

    public override string ToString()
    {
        return $"{base.ToString()} + Spicy";
    }
}
