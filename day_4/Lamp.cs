namespace CarComponent;

class Lamp

{
	private string _lampType = "";
	
	public Lamp(string type)
	
	{
		_lampType = type;
	}
	public void TurnOn()
	
	{
		Console.WriteLine($"{_lampType} Light On");
	}
}
