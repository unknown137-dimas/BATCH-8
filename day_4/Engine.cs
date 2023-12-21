namespace CarComponent;
class Engine

{
	private int _cylinderCount;
	private bool _turbo;
	public Engine(int cylinderNumber, bool turbo = false)
	
	{
		_cylinderCount = cylinderNumber;
		_turbo = turbo;
	}
	public virtual void Run()
	
	{
		Console.WriteLine(_turbo ? $"{_cylinderCount} cylinder engine with turbo is running..." : $"{_cylinderCount} cylinder engine is running...");
	}
}