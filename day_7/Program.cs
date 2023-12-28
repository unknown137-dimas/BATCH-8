class Program

{
	static void Main()
	{
		// Delegate
		Console.WriteLine("\nDelegate");
		Youtuber youtuber1 = new("Mr. Beast");
		Subscriber sub1 = new("Dimas");
		Subscriber sub2 = new("Fitrio");
		
		youtuber1.AddSubscriber(sub1.GetNotification);
		youtuber1.AddSubscriber(sub2.GetNotification);
		youtuber1.UploadVideo("Free Money");
		
		// EventHandler
		Console.WriteLine("\nEventHandler");
		Youtuber youtuber2 = new("GadgetIn");
		youtuber2.mySub += sub1.GetNotification;
		youtuber2.UploadVideo("Review Iphone 16 Pro Max");
	}
}