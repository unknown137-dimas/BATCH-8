class Subscriber
{
	string name;
	public Subscriber(string name) => this.name = name;
	public void GetNotification(string sender, string message) => Console.WriteLine($"Hi {name}, {sender} send notification : {message}");
	public void GetNotification(object sender, EventArgs e) => Console.WriteLine($"Hi {name}, {sender.ToString()} send notification");
}