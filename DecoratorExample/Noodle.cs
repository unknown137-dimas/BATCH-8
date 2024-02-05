namespace DecoratorExample;

public class Noodle : INoodle
{
    public virtual int Cost() => 5000;
    public override string ToString() => "Noodle";
}
