namespace CarComponent;

class ElectricMotor : Engine

{
	private int _motorCount;
	public ElectricMotor(int motorNumber): base(0, false)
	
	{
		_motorCount = motorNumber;
	}
    public override void Run()
    {
        Console.WriteLine($"Car is running and powered by {_motorCount} electric motors.");
    }
}