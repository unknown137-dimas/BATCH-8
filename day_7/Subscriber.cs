class Subscriber
{
	string name;
	public Subscriber(string name) => this.name = name;
	public void GetNotification(string sender, string message) => Console.WriteLine($"Delegate : Hi {name}, {sender} send notification : {message}");
	public void GetNotification(object sender, EventArgs e) => Console.WriteLine($"EventHandler : Hi {name}, {sender.ToString()} send notification");
	public void GetNotification(object sender, YoutuberEventArgs e) => Console.WriteLine($"EventHandler<T> : Hi {name}, {sender.ToString()} send notification : {e.message}");
	public void GetNotification(string message, bool code) => Console.WriteLine($"Action : Hi {name}, send notification : {message} {code}");
}