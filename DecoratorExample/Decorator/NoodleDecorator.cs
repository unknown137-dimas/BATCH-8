namespace DecoratorExample;

public class NoodleDecorator : INoodle
{
    protected readonly INoodle _noodle;
    public NoodleDecorator(INoodle noodle)
    {
        _noodle = noodle;
    }
    public virtual int Cost() => _noodle.Cost();
    public override string ToString() => $"{_noodle}";
}
