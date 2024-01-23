using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

class Program
{
	static void Main()
	{
		// StreamWriter
		string path = @"output.txt";
		FileStreamOptions options = new()
		{
			Mode = FileMode.OpenOrCreate,
			Access = FileAccess.Write,
			Share = FileShare.None
		};
		// using(FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
		// {
		// }
		using(StreamWriter writer = new(path, options))
		{
			writer.WriteLine("Annyeonghaseyo");
		}
		
		// StreamReader
		// using(FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Read))
		// {
		// }
		using(StreamReader reader = new(path))
		{
			Console.WriteLine(reader.ReadToEnd());
		}
		
		// Serializer & Deserializer
		
		// Data
		Dictionary<string, Hero> heroDb = new();
		heroDb.Add("Dimas", new Hero("Dimas", 100, 20));
		heroDb.Add("Fitrio", new Hero("Fitrio", 200, 30));
		heroDb.Add("Kurniawan", new Hero("Kurniawan", 300, 40));
		
		
		// Output data
		Dictionary<string, Hero> result = new();
		
		// JSON Serializer & Deserializer
		string jsonPath = @"hero.json";
		DataContractJsonSerializer jsonSerializer = new(typeof(Dictionary<string, Hero>));
		using(FileStream fs = new(jsonPath, FileMode.Create))
		{
			
			// Write object to json using JsonReaderWriterFactory
			using(var jsonFactory = JsonReaderWriterFactory.CreateJsonWriter(fs, Encoding.UTF8, true, true, "  "))
			{
				jsonSerializer.WriteObject(jsonFactory, heroDb);
			}
			
		}
		using(FileStream fs = new(jsonPath, FileMode.Open))
		{
			// Read the json file
			result = (Dictionary<string, Hero>)jsonSerializer.ReadObject(fs);
		}
		
		foreach(var hero in result)
		{
			Console.WriteLine($"{hero.Value.Name} | HP:{hero.Value.Hp}, ATK:{hero.Value.Attack}");
		}
		
	}
}

[DataContract]
struct Hero
{
	[DataMember]
	public string Name {get; private set;}
	[DataMember]
	public int Hp {get; private set;}
	[DataMember]
	public int Attack {get; private set;}
	
	public Hero(string name, int hp, int attack)
	{
		Name = name;
		Hp = hp;
		Attack = attack;
	}
}