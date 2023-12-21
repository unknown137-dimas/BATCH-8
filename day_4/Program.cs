using System.Diagnostics;
using System.Text;
using CarComponent;

class Program

{
	static void Main()
	
	{
		// #1 string
		string a = "Hello";
		string b = a;
		b += " World";
		Console.WriteLine(a);
		Console.WriteLine(b);
		
		// #2 StringBuilder vs string
		// Demo perbedaan performa dari string dan StringBuilder (Array in a nutshell)
		StringBuilder sbWord = new();
		string stringWord = "";
		Stopwatch sbStopWatch = new();
		Stopwatch stringStopWatch = new();
		
		int iter = 50000;
		
		sbStopWatch.Start();
		for(int i = 0; i < iter; i++)
		
		{
			sbWord.Append(":) ");
		}
		sbStopWatch.Stop();
		
		stringStopWatch.Start();
		for(int i = 0; i < iter; i++)
		
		{
			stringWord += ":) ";
		}
		stringStopWatch.Stop();
		
		Console.WriteLine($"StringBuilder Time : {sbStopWatch.ElapsedMilliseconds} ms");
		Console.WriteLine($"string Time : {stringStopWatch.ElapsedMilliseconds} ms");
		
		// #3 readonly
		Car car = new(new Engine(4), new Lamp("bulp"));
		// car.years bisa dirubah walaupun readonly karena yang readonly itu arraynya bukan tipe data dari car.years.
		car.years[0] = 2000;
		car.DisplayYears();
		// car.years = new {1,2,3,4,5}; // <= Muncul error karena readonly, merubah array dan merubah banyak data sekaligus.
		for(int i = 0; i < car.years.Length; i++) // <= Tidak muncul error karena merubah data satu2 dan tidak mengganti array.
		
		{
			car.years[i] = 2010 + i;
		}
		car.DisplayYears();
		
		// #4 Object Composition
		Car oldCar = new(new Engine(4), new Lamp("bulp"));
		Console.WriteLine("Car brooom.... brooom... ");
		oldCar.Start();
		Car newCar = oldCar;
		newCar.SwapEngine(new Engine(8, true));
		newCar.ChangeLamp(new Lamp("LED"));
		newCar.Start();
		Car electricCar = newCar;
		electricCar.SwapEngine(new ElectricMotor(3));
		newCar.Start();
		
	}
}