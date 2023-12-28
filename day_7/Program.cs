class Program

{
	static void Main()
	{
		Subscriber sub1 = new("Dimas");
		Subscriber sub2 = new("Fitrio");
		
		// Delegate
		Console.WriteLine("\nDelegate");
		Youtuber youtuber1 = new("Mr. Beast");
		youtuber1.AddSubscriber(sub1.GetNotification);
		youtuber1.AddSubscriber(sub2.GetNotification);
		youtuber1.UploadVideo("Free Money");
		
		// EventHandler
		Console.WriteLine("\nEventHandler");
		Youtuber youtuber2 = new("GadgetIn");
		youtuber2.mySub += sub1.GetNotification;
		youtuber2.myEventHandler += sub2.GetNotification;
		youtuber2.UploadVideo("Review Iphone 16 Pro Max");
		
		// Action
		Console.WriteLine("\nAction");
		Youtuber youtuber3 = new("Jagat Review");
		youtuber3.myAction += sub1.GetNotification;
		youtuber3.UploadVideo("Review Macbook Pro Gaming");
		
	}
}