using System.Collections.Immutable;

class Program

{
	static void Main()
	
	{
		// Enum
		Console.WriteLine($"DOB : {DaysOfWeek.Monday} 25 {Month.December} 2000");
		string dayOfWeek = DayOfWeek.Sunday.ToString();
		Console.WriteLine(dayOfWeek);
		
		//Single Responsibility Principle
		//Single class only have single purpose
		//Single method only have single purpose
		Console.WriteLine();
		Human human1 = new("Dimas FK", DayOfWeek.Monday, 25, Month.December, 2000);
		var dob = human1.GetDOB();
		DayOfWeek day = dob.Item1;
		int date = dob.Item2;
		Month month = dob.Item3;
		int year = dob.Item4;
		Console.WriteLine($"Nama       : {human1.GetName()}");
		Console.WriteLine($"Lahir pada : {day}, {date} {month} {year}");
		
		// Collections
		Console.WriteLine();
		List<int> yearList = new(); // <T> to maintain typesafety
		yearList.AddRange(new[]{2000, 2001, 2002});
		yearList.AddRange(new[]{2003, 2004, 2020});
		yearList.ForEach(Console.WriteLine);
		
		// Method Extensions
		Console.WriteLine();
		human1.GetName().Dump();
		int result = 2999.Tambah(1);
		result.Dump();
	}
}

