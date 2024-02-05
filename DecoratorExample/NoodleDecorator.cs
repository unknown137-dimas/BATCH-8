namespace DecoratorExample;

public abstract class NoodleDecorator : INoodle
{
	protected readonly INoodle _noodle;

	public NoodleDecorator(INoodle noodle)
	{
		_noodle = noodle;
	}

	public virtual int Cost()
	{
		return _noodle.Cost();
	}

    public override string ToString()
    {
        return $"{_noodle.ToString()}";
    }

}
