class Progam
{
	static void Main()
	{
		// Abstraction
		Animal cat = new Cat(9, "Garfield");
		cat.Eat("Lasagna");
		cat.MakeSound();
		cat.SayHi();
		Console.WriteLine();
		
		// Interface
		FeaturePhone featurePhone = new("Nokia", "3310");
		featurePhone.GetPhoneModel();
		featurePhone.Call("Ayang");
		featurePhone.Message("I love u", "Ayang");
		Console.WriteLine();
		
		SmartPhone gamingPhone = new("Asus", "ROG 6");
		gamingPhone.GetPhoneModel();
		gamingPhone.PlayGames("Ganshin Impact");
		gamingPhone.Call("Nobody");
		gamingPhone.Message("Pinjem dulu seratus", "Bro Mobile");
	}
}