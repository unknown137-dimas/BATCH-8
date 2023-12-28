class Program

{
	static void Main()
	{
		// Delegate
		Youtuber youtuber1 = new("Mr. Beast");
		Subscriber sub1 = new("Dimas");
		Subscriber sub2 = new("Fitrio");
		
		youtuber1.AddSubscriber(sub1.GetNotification);
		youtuber1.AddSubscriber(sub2.GetNotification);
		youtuber1.UploadVideo("Free Money");
	}
}