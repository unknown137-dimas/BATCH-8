namespace CarComponent;

class ElectricMotor : Engine

{
	private int MotorCount {get; set; }
	public ElectricMotor(int motorNumber): base(0, false) => MotorCount = motorNumber;
    public override void Run() => Console.WriteLine($"Car is running and powered by {MotorCount} electric motors.");
}