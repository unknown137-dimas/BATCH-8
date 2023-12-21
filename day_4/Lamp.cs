namespace CarComponent;

class Lamp

{
	private string LampType{ get; set; }
	
	public Lamp(string type) => LampType = type;
	public void TurnOn() => Console.WriteLine($"{LampType} Light On");
}
