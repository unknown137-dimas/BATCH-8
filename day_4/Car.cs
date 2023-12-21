using CarComponent;

class Car

{
	public readonly int[] years = new int[5];
	public Engine carEngine;
	public Lamp carLamp;
	
	public Car(Engine engine, Lamp lamp)
	
	{
		carEngine = engine;
		carLamp = lamp;
	}
	
	public void Start()
	
	{
		carEngine.Run();
		carLamp.TurnOn();
	}
	
	public void SwapEngine(Engine newEngine)
	
	{
		carEngine = newEngine;
	}
	public void ChangeLamp(Lamp newLamp)
	
	{
		carLamp = newLamp;
	}
	public void DisplayYears()
	
	{
		Console.WriteLine("===Data Tahun Mobil===");
		int i = 0;
		foreach(int year in years)
		
		{
			Console.WriteLine($"index {i++} : {year}");
		}
	}
}