public delegate void MySubscriber(string name, string message); // delegate
class Youtuber
{
	private event MySubscriber? myDel; // Delegate
	public event EventHandler? mySub; // EventHandler a.k.a. Internal delegate from Microsoft
	public event EventHandler<YoutuberEventArgs>? myEventHandler; // EventHandler with parameter;
	public string Name { get; }
	public Youtuber(string name) => Name = name;
	public void UploadVideo(string title)
	{
		Console.WriteLine($"Uploading {title} video...");
		Console.WriteLine($"{title} uploaded...");
		SendNotification(Name, title);
		SendNotification();
		SendNotification(title);
	}
	public void SendNotification(string name, string message) => myDel?.Invoke(Name, message);
	public void SendNotification() => mySub?.Invoke(this, EventArgs.Empty);
	public void SendNotification(string message) => myEventHandler?.Invoke(this, new YoutuberEventArgs() {message=message});
	public override string ToString() => Name;
	public bool AddSubscriber(MySubscriber sub)
	{
		if(myDel is null || !myDel.GetInvocationList().Contains(sub))
		{
			myDel += sub;
			return true;
		}
		return false;
	}
}