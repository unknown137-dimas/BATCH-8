namespace DecoratorExample;

public class Noodle : INoodle
{
	public virtual int Cost()
	{
		return 5000;
	}

    public override string ToString()
    {
        return "Noodle";
    }
}
