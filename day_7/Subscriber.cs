class Subscriber
{
	string name;
	public Subscriber(string name) => this.name = name;
	public void GetNotification(string youtuber, string message) => Console.WriteLine($"Hi {name}, {youtuber} send notification : {message}");
}