class FeaturePhone : Phone
{
	private string brand;
	private string model;
	private static int count;
	
	public FeaturePhone(string brand, string model)
	
	{
		this.brand = brand;
		this.model = model;
		count++;
	}
	public void GetPhoneModel() => Console.WriteLine($"{brand} {model}");
	public override void Call(string contactNumber)
	{
		Console.WriteLine($"Calling {contactNumber}...");
	}

	public override void Message(string message, string contactNumber)
	{
		Console.WriteLine($"Say {message} to {contactNumber}");
	}
	public static int GetTotal() => count;
}