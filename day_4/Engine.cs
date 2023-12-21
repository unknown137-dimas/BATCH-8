namespace CarComponent;
class Engine

{
	private int CylinderCount { get; set; }
	private bool Turbo { get; set; }
	public Engine(int cylinderNumber, bool turbo = false)
	
	{
		CylinderCount = cylinderNumber;
		Turbo = turbo;
	}
	public virtual void Run()
	
	{
		Console.WriteLine(Turbo ? $"{CylinderCount} cylinder engine with turbo is running..." : $"{CylinderCount} cylinder engine is running...");
	}
}