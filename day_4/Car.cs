using CarComponent;

class Car

{
	public int[] years = new int[5];
	public Engine CarEngine { get; set; } 
	public Lamp CarLamp { get; set; }
	
	public Car(Engine engine, Lamp lamp)
	
	{
		CarEngine = engine;
		CarLamp = lamp;
	}
	
	public void Start()
	
	{
		CarEngine.Run();
		CarLamp.TurnOn();
	}
	
	public void SwapEngine(Engine newEngine) => CarEngine = newEngine;
	public void ChangeLamp(Lamp newLamp) => CarLamp = newLamp;
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