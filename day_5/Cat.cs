class Cat : Animal
{
	public Cat(int age, string name) : base(age, name) {}
	public override void MakeSound() => Console.WriteLine("Meow... Meow...");
    public override void SayHi() => Console.WriteLine($"i am {name}, nice to meet you :)");
}